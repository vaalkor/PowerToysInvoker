namespace PowerToysInvoker
{
    partial class PowerToysInvoker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PowerToysInvoker));
            this.TextExtractor = new System.Windows.Forms.Button();
            this.AlwaysOnTop = new System.Windows.Forms.Button();
            this.ColourPicker = new System.Windows.Forms.Button();
            this.FancyZones = new System.Windows.Forms.Button();
            this.ScreenRuler = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextExtractor
            // 
            this.TextExtractor.Location = new System.Drawing.Point(2, 32);
            this.TextExtractor.Name = "TextExtractor";
            this.TextExtractor.Size = new System.Drawing.Size(182, 23);
            this.TextExtractor.TabIndex = 0;
            this.TextExtractor.Text = "Text Extractor";
            this.TextExtractor.UseVisualStyleBackColor = true;
            this.TextExtractor.Click += new System.EventHandler(this.TextExtractor_Click);
            // 
            // AlwaysOnTop
            // 
            this.AlwaysOnTop.Location = new System.Drawing.Point(2, 3);
            this.AlwaysOnTop.Name = "AlwaysOnTop";
            this.AlwaysOnTop.Size = new System.Drawing.Size(182, 23);
            this.AlwaysOnTop.TabIndex = 1;
            this.AlwaysOnTop.Text = "Always On Top";
            this.AlwaysOnTop.UseVisualStyleBackColor = true;
            this.AlwaysOnTop.Click += new System.EventHandler(this.AlwaysOnTop_Click);
            // 
            // ColourPicker
            // 
            this.ColourPicker.Location = new System.Drawing.Point(2, 61);
            this.ColourPicker.Name = "ColourPicker";
            this.ColourPicker.Size = new System.Drawing.Size(182, 23);
            this.ColourPicker.TabIndex = 2;
            this.ColourPicker.Text = "ColourPicker";
            this.ColourPicker.UseVisualStyleBackColor = true;
            this.ColourPicker.Click += new System.EventHandler(this.ColourPicker_Click);
            // 
            // FancyZones
            // 
            this.FancyZones.Location = new System.Drawing.Point(2, 119);
            this.FancyZones.Name = "FancyZones";
            this.FancyZones.Size = new System.Drawing.Size(182, 23);
            this.FancyZones.TabIndex = 3;
            this.FancyZones.Text = "Fancy Zones";
            this.FancyZones.UseVisualStyleBackColor = true;
            this.FancyZones.Click += new System.EventHandler(this.FancyZones_Click);
            // 
            // ScreenRuler
            // 
            this.ScreenRuler.Location = new System.Drawing.Point(2, 90);
            this.ScreenRuler.Name = "ScreenRuler";
            this.ScreenRuler.Size = new System.Drawing.Size(182, 23);
            this.ScreenRuler.TabIndex = 4;
            this.ScreenRuler.Text = "Screen Ruler";
            this.ScreenRuler.UseVisualStyleBackColor = true;
            this.ScreenRuler.Click += new System.EventHandler(this.ScreenRuler_Click);
            // 
            // PowerToysInvoker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(187, 147);
            this.Controls.Add(this.ScreenRuler);
            this.Controls.Add(this.FancyZones);
            this.Controls.Add(this.ColourPicker);
            this.Controls.Add(this.AlwaysOnTop);
            this.Controls.Add(this.TextExtractor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PowerToysInvoker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PToys";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button TextExtractor;
        private System.Windows.Forms.Button AlwaysOnTop;
        private System.Windows.Forms.Button ColourPicker;
        private System.Windows.Forms.Button FancyZones;
        private System.Windows.Forms.Button ScreenRuler;
    }
}

