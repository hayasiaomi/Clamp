using System.Drawing;

namespace Clamp.MUI
{
    partial class FrmError
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
            this.TopBar = new System.Windows.Forms.Panel();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.pbMinimum = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnReset = new Clamp.MUI.NoFocusButton();
            this.pbErrorImage = new System.Windows.Forms.PictureBox();
            this.lblError = new System.Windows.Forms.Label();
            this.TopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbErrorImage)).BeginInit();
            this.SuspendLayout();
            // 
            // TopBar
            // 
            this.TopBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(172)))), ((int)(((byte)(132)))));
            this.TopBar.Controls.Add(this.pbClose);
            this.TopBar.Controls.Add(this.pbMinimum);
            this.TopBar.Controls.Add(this.lblTitle);
            this.TopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopBar.Location = new System.Drawing.Point(0, 0);
            this.TopBar.Name = "TopBar";
            this.TopBar.Size = new System.Drawing.Size(608, 40);
            this.TopBar.TabIndex = 0;
            this.TopBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseDown);
            this.TopBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopBar_MouseMove);
            // 
            // pbClose
            // 
            this.pbClose.Location = new System.Drawing.Point(544, 7);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(28, 27);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbClose.TabIndex = 1;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            // 
            // pbMinimum
            // 
            this.pbMinimum.Location = new System.Drawing.Point(507, 7);
            this.pbMinimum.Name = "pbMinimum";
            this.pbMinimum.Size = new System.Drawing.Size(28, 27);
            this.pbMinimum.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbMinimum.TabIndex = 1;
            this.pbMinimum.TabStop = false;
            this.pbMinimum.Click += new System.EventHandler(this.pbMinimum_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(13, 9);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(24, 11, 3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(76, 21);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "label1";
            // 
            // btnReset
            // 
            this.btnReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(221, 344);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(166, 53);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "button1";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // pbErrorImage
            // 
            this.pbErrorImage.Location = new System.Drawing.Point(90, 96);
            this.pbErrorImage.Name = "pbErrorImage";
            this.pbErrorImage.Size = new System.Drawing.Size(81, 76);
            this.pbErrorImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbErrorImage.TabIndex = 2;
            this.pbErrorImage.TabStop = false;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblError.ForeColor = System.Drawing.Color.Black;
            this.lblError.Location = new System.Drawing.Point(173, 117);
            this.lblError.Margin = new System.Windows.Forms.Padding(0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(76, 21);
            this.lblError.TabIndex = 3;
            this.lblError.Text = "label1";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FrmError
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(608, 440);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.pbErrorImage);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.TopBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmError";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmError";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FrmError_Shown);
            this.TopBar.ResumeLayout(false);
            this.TopBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbErrorImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel TopBar;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox pbErrorImage;
        private System.Windows.Forms.PictureBox pbMinimum;
        private System.Windows.Forms.PictureBox pbClose;
        private System.Windows.Forms.Label lblError;
        private NoFocusButton btnReset;
    }
}