using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using Clamp.MUI.Properties;

namespace Clamp.MUI.Splash
{
    /// <summary>
    /// 开始的加载界面
    /// </summary>
    internal class FrmSplash : Form
    {
        private PictureBox pboxSplashImage;
        private Label lblVersionTitle;
        private Label lblSplashText;

        public FrmSplash()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.pboxSplashImage = new PictureBox();
            this.lblVersionTitle = new Label();
            this.lblSplashText = new Label();

            this.Width = 442;
            this.Height = 250;
            this.TopMost = true;

            ((System.ComponentModel.ISupportInitialize)(this.pboxSplashImage)).BeginInit();

            this.SuspendLayout();

            this.pboxSplashImage.Image = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("Clamp.MUI.Resources.Splash.png"));
            this.pboxSplashImage.Width = 60;
            this.pboxSplashImage.Height = 60;
            this.pboxSplashImage.SizeMode = PictureBoxSizeMode.CenterImage;

            this.lblVersionTitle.Font = new Font(SystemFonts.DefaultFont.FontFamily, 14f);
            this.lblVersionTitle.AutoSize = true;
            this.lblVersionTitle.Text = StringResources.FrmSplash_VersionTitle;
            this.lblVersionTitle.ForeColor = Color.White;

            this.lblSplashText.Font = new Font(SystemFonts.DefaultFont.FontFamily, 14f);
            this.lblSplashText.AutoSize = true;
            this.lblSplashText.ForeColor = Color.White;

            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = ColorTranslator.FromHtml("#3bac84");


            this.Controls.Add(this.pboxSplashImage);
            this.Controls.Add(this.lblVersionTitle);
            this.Controls.Add(this.lblSplashText);

            ((System.ComponentModel.ISupportInitialize)(this.pboxSplashImage)).EndInit();
            this.ResumeLayout(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.pboxSplashImage.Location = new Point((this.Width - this.pboxSplashImage.Width) / 2, 48);

            this.lblVersionTitle.Location = new Point((this.Width - this.lblVersionTitle.Width) / 2, this.pboxSplashImage.Location.Y + this.pboxSplashImage.Height + 10);

            this.lblSplashText.Location = new Point(10, this.Height - this.lblSplashText.Height - 10);

            Thread splashThread = new Thread(new ThreadStart(this.InitializeEnvironment));
            splashThread.Start();
        }

        private void InitializeEnvironment()
        {

            

            this.SetSplashText(StringResources.FrmSplash_OpenApplication);


            Thread.Sleep(2000);

            ChromiumSettings.SplashResult = new SplashResult()
            {
                Completed = true,
                ErrorMessage = "Test"
            };

            this.Invoke(new Action(() =>
            {
                this.Close();
            }));


        }


        private void SetSplashText(string text)
        {
             this.Invoke(new Action(() =>
            {
                this.lblSplashText.Text = text; 
            }));
        }
    }
}
