namespace Clamp.Upgrader
{
    partial class FrmMain
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
            this.blackStyleButton1 = new Clamp.Upgrader.Controls.BlackStyleButton();
            this.SuspendLayout();
            // 
            // blackStyleButton1
            // 
            this.blackStyleButton1.Font = new System.Drawing.Font("Arial", 12F);
            this.blackStyleButton1.ForeColor = System.Drawing.Color.White;
            this.blackStyleButton1.Location = new System.Drawing.Point(42, 203);
            this.blackStyleButton1.Name = "blackStyleButton1";
            this.blackStyleButton1.Size = new System.Drawing.Size(265, 48);
            this.blackStyleButton1.TabIndex = 0;
            this.blackStyleButton1.Text = "升级";
            this.blackStyleButton1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.blackStyleButton1_MouseClick);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.ClientSize = new System.Drawing.Size(358, 263);
            this.Controls.Add(this.blackStyleButton1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "升级";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.BlackStyleButton blackStyleButton1;
    }
}