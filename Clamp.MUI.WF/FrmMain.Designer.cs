﻿namespace Clamp.MUI.WF
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ChromiumWebBrowser = new Chromium.WebBrowser.ChromiumWebBrowser();
            this.SuspendLayout();
            // 
            // ChromiumWebBrowser
            // 
            this.ChromiumWebBrowser.BackColor = System.Drawing.Color.White;
            this.ChromiumWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChromiumWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.ChromiumWebBrowser.Name = "ChromiumWebBrowser";
            this.ChromiumWebBrowser.RemoteCallbackInvokeMode = Chromium.WebBrowser.JSInvokeMode.Inherit;
            this.ChromiumWebBrowser.Size = new System.Drawing.Size(1024, 680);
            this.ChromiumWebBrowser.TabIndex = 0;
            this.ChromiumWebBrowser.Text = "chromiumWebBrowser1";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 680);
            this.Controls.Add(this.ChromiumWebBrowser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Chromium.WebBrowser.ChromiumWebBrowser ChromiumWebBrowser;
    }
}

