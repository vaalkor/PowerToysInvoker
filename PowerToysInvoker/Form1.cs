using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace PowerToysInvoker
{
    public partial class PowerToysInvoker : Form
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr GetClickedWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(uint uAction, uint uParam, IntPtr lpvParam, uint fuWinIni);

        // P/Invoke declarations for hook management
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(System.Drawing.Point pt);


        // Mouse hook struct to access the mouse event information
        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public System.Drawing.Point pt;
            public int mouseData;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam); // Delegate for the hook procedure
        private IntPtr _mouseHookHandle = IntPtr.Zero; // Store the hook handle

        // Constants for mouse hooks
        const int WH_MOUSE_LL = 14; // Low-level mouse hook type
        const int WM_LBUTTONDOWN = 0x0201; // Left mouse button down
        const int WM_RBUTTONDOWN = 0x0204; // Right mouse button down

        // Constants for setting the cursor
        private const uint SPI_SETCURSORS = 0x0057;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;
        const uint OCR_NORMAL_CURSOR = 32512;
        private IntPtr _originalCursorHandle = IntPtr.Zero;

        private static bool isEyedropperActive = false;

        // Constants for the key event flags
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        private static Dictionary<string, KeyConfiguration> PowerToysHotKeys = new Dictionary<string, KeyConfiguration>();

        private static class PowerToyNames
        {
            public static string FancyZones = "FancyZones";
            public static string AlwaysOnTop = "AlwaysOnTop";
            public static string MeasureTool = "Measure Tool";
            public static string TextExtractor = "TextExtractor";
            public static string ColorPicker = "ColorPicker";
        }

        public PowerToysInvoker()
        {
            InitializeComponent();
            CenterFormOnMouse();
            ReadPowertoysConfigFiles();
            InstallMouseHook();
            KeyPreview = true;
            KeyDown += Form_KeyDown;
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private static void PressControlKeys(KeyConfiguration config)
        {
            if (config.Win)
                keybd_event(0x5B, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            if (config.Ctrl)
                keybd_event(0x11, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            if (config.Alt)
                keybd_event(0x12, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            if (config.Shift)
                keybd_event(0x10, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        }

        private static void ReleaseControlKeys(KeyConfiguration config)
        {
            if (config.Shift)
                keybd_event(0x10, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            if (config.Alt)
                keybd_event(0x12, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            if (config.Ctrl)
                keybd_event(0x11, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            if (config.Win)
                keybd_event(0x5B, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private static void PressAndReleaseControlKeyForPowerToy(string powerToyName)
        {
            var config = PowerToysHotKeys[powerToyName];
            PressControlKeys(config);
            keybd_event(Convert.ToByte(config.Code), 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            ReleaseControlKeys(config);
        }

        /// <summary>
        /// Centers the form on the current mouse position, but clamps if it would cross any boundaries.
        /// </summary>
        private void CenterFormOnMouse()
        {
            Screen currentScreen = Screen.FromPoint(Cursor.Position);

            var clampedX = Utils.Clamp(Cursor.Position.X - Width / 2, 0, currentScreen.WorkingArea.Width - Width);
            var clampedY = Utils.Clamp(Cursor.Position.Y - Height / 2, 0, currentScreen.WorkingArea.Height - Height);
            
            Left = clampedX;
            Top = clampedY;
        }

        private void TextExtractor_Click(object sender, EventArgs e)
        {
            Hide();
            PressAndReleaseControlKeyForPowerToy(PowerToyNames.TextExtractor);
            Close();
        }

        private void ColourPicker_Click(object sender, EventArgs e)
        {
            Hide();
            PressAndReleaseControlKeyForPowerToy(PowerToyNames.ColorPicker);
            Close();
        }

        private void FancyZones_Click(object sender, EventArgs e)
        {
            Hide();
            PressAndReleaseControlKeyForPowerToy(PowerToyNames.FancyZones);
            Close();
        }

        private void ScreenRuler_Click(object sender, EventArgs e)
        {
            Hide();
            PressAndReleaseControlKeyForPowerToy(PowerToyNames.MeasureTool);
            Close();
        }


        private void AlwaysOnTop_Click(object sender, EventArgs e)
        {
            if (!isEyedropperActive)
            {
                Hide();
                isEyedropperActive = true;
            }
        }

        private void ReadPowertoysConfigFiles()
        {
            Dictionary<string, Func<JObject, KeyConfiguration>> ConfigRetrievalFunctions = new Dictionary<string, Func<JObject, KeyConfiguration>>
            {
                { PowerToyNames.FancyZones, (jsonObject) => jsonObject["properties"]["fancyzones_editor_hotkey"]["value"].ToObject<KeyConfiguration>() },
                { PowerToyNames.AlwaysOnTop, (jsonObject) => jsonObject["properties"]["hotkey"]["value"].ToObject<KeyConfiguration>()},
                { PowerToyNames.MeasureTool, (jsonObject) => jsonObject["properties"]["ActivationShortcut"].ToObject<KeyConfiguration>()},
                { PowerToyNames.TextExtractor, (jsonObject) => jsonObject["properties"]["ActivationShortcut"].ToObject<KeyConfiguration>()},
                { PowerToyNames.ColorPicker, (jsonObject) => jsonObject["properties"]["ActivationShortcut"].ToObject<KeyConfiguration>()}
            };

            foreach (var powerToyName in ConfigRetrievalFunctions.Keys)
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string powerToyPath = Path.Combine(appDataPath, $"Microsoft\\PowerToys\\{powerToyName}\\settings.json");

                if (!File.Exists(powerToyPath))
                    ErrorAndClose($"Could not find file: \"{powerToyPath}\"");

                string json = File.ReadAllText(powerToyPath);
                JObject configFile = JObject.Parse(json);

                PowerToysHotKeys[powerToyName] = ConfigRetrievalFunctions[powerToyName](configFile);
            }
        }

        private void InstallMouseHook()
        {
            HookProc hookProc = new HookProc(MouseHookCallback);
            IntPtr hInstance = GetModuleHandle(null); // Get the instance handle of the current process
            _mouseHookHandle = SetWindowsHookEx(WH_MOUSE_LL, hookProc, hInstance, 0);

            if (_mouseHookHandle == IntPtr.Zero)
            {
                ErrorAndClose("Failed to set mouse hook!");
            }
        }

        // Callback method for the mouse hook
        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (!isEyedropperActive || nCode < 0)
                return CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);


            // Check if the event is a left mouse button
            if (wParam == (IntPtr)WM_LBUTTONDOWN)
            {
                Console.WriteLine("Left mouse button clicked!");

                // Convert lParam to MSLLHOOKSTRUCT to access mouse data
                MSLLHOOKSTRUCT mouseHookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                System.Drawing.Point clickPosition = mouseHookStruct.pt;

                IntPtr hwndClicked = WindowFromPoint(clickPosition);

                if (hwndClicked == Handle)
                    return CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam); // If we've clicked on ourself, ignore the click.

                if (hwndClicked != IntPtr.Zero)
                {
                    SetForegroundWindow(hwndClicked);

                    PressAndReleaseControlKeyForPowerToy(PowerToyNames.AlwaysOnTop);
                }

                Close();
            }

            // Pass the message to the next hook
            return CallNextHookEx(_mouseHookHandle, nCode, wParam, lParam);
        }

        // Unhook the mouse hook when the form is closed
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            UnhookWindowsHookEx(_mouseHookHandle);
        }

        private void ErrorAndClose(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
    }
}
