namespace Clamp.MUI.WF.Controls
{
    partial class FrmBase
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
            this.chromium = new Chromium.WebBrowser.ChromiumWebBrowser();
            this.SuspendLayout();
            // 
            // ChromiumWebBrowser
            // 
            this.chromium.BackColor = System.Drawing.Color.White;
            this.chromium.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chromium.Location = new System.Drawing.Point(0, 0);
            this.chromium.Name = "ChromiumWebBrowser";
            this.chromium.RemoteCallbackInvokeMode = Chromium.WebBrowser.JSInvokeMode.Inherit;
            this.chromium.Size = new System.Drawing.Size(800, 450);
            this.chromium.TabIndex = 0;
            this.chromium.Text = "chromiumWebBrowser1";
            // 
            // FrmBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.chromium);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmBase";
            this.ResumeLayout(false);

        }

        #endregion

        private Chromium.WebBrowser.ChromiumWebBrowser chromium;
    }
}