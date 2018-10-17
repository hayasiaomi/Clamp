using System.Windows.Forms;

namespace Clamp.UIShell.Forms
{
    partial class FrmSplash : Form
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pboxImage = new System.Windows.Forms.PictureBox();
            this.pbarInitalized = new System.Windows.Forms.ProgressBar();
            this.lblLogoTitle = new System.Windows.Forms.Label();
            this.lblProductTitle = new System.Windows.Forms.Label();
            this.pboxLogo = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(172)))), ((int)(((byte)(132)))));
            this.panel1.Controls.Add(this.pboxImage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(376, 440);
            this.panel1.TabIndex = 2;
            // 
            // pboxImage
            // 
            this.pboxImage.Image = global::Clamp.UIShell.Properties.SDResources.u37;
            this.pboxImage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pboxImage.Location = new System.Drawing.Point(32, 48);
            this.pboxImage.Name = "pboxImage";
            this.pboxImage.Size = new System.Drawing.Size(315, 310);
            this.pboxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pboxImage.TabIndex = 0;
            this.pboxImage.TabStop = false;
            // 
            // pbarInitalized
            // 
            this.pbarInitalized.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pbarInitalized.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(172)))), ((int)(((byte)(132)))));
            this.pbarInitalized.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pbarInitalized.Location = new System.Drawing.Point(0, 550);
            this.pbarInitalized.Name = "pbarInitalized";
            this.pbarInitalized.Size = new System.Drawing.Size(376, 10);
            this.pbarInitalized.Step = 1;
            this.pbarInitalized.TabIndex = 3;
            // 
            // lblLogoTitle
            // 
            this.lblLogoTitle.AutoSize = true;
            this.lblLogoTitle.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold);
            this.lblLogoTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(172)))), ((int)(((byte)(132)))));
            this.lblLogoTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblLogoTitle.Location = new System.Drawing.Point(178, 471);
            this.lblLogoTitle.Name = "lblLogoTitle";
            this.lblLogoTitle.Size = new System.Drawing.Size(68, 27);
            this.lblLogoTitle.TabIndex = 5;
            this.lblLogoTitle.Text = "善点";
            // 
            // lblProductTitle
            // 
            this.lblProductTitle.AutoSize = true;
            this.lblProductTitle.Font = new System.Drawing.Font("宋体", 14F);
            this.lblProductTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.lblProductTitle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblProductTitle.Location = new System.Drawing.Point(49, 514);
            this.lblProductTitle.Name = "lblProductTitle";
            this.lblProductTitle.Size = new System.Drawing.Size(276, 19);
            this.lblProductTitle.TabIndex = 6;
            this.lblProductTitle.Text = "贴地气的/餐厅信息化/智能工具";
            // 
            // pboxLogo
            // 
            this.pboxLogo.Image = global::Clamp.UIShell.Properties.SDResources.u32;
            this.pboxLogo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pboxLogo.Location = new System.Drawing.Point(136, 467);
            this.pboxLogo.Name = "pboxLogo";
            this.pboxLogo.Size = new System.Drawing.Size(35, 35);
            this.pboxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pboxLogo.TabIndex = 4;
            this.pboxLogo.TabStop = false;
            // 
            // FrmSplash
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(376, 560);
            this.Controls.Add(this.lblProductTitle);
            this.Controls.Add(this.lblLogoTitle);
            this.Controls.Add(this.pboxLogo);
            this.Controls.Add(this.pbarInitalized);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmSplash";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pboxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        private Panel panel1;
        private PictureBox pboxImage;
        private ProgressBar pbarInitalized;
        private PictureBox pboxLogo;
        private Label lblLogoTitle;
        private Label lblProductTitle;
    }
}