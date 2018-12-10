using Clamp.MUI.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.MUI
{
    public partial class FrmError : Form
    {
        private Point dragOffset = new Point();

        public string Error { private set; get; }

        public FrmError(string error)
        {
            this.Error = error;

            InitializeComponent();
        }

        private void FrmError_Shown(object sender, EventArgs e)
        {
            this.lblError.Text = this.Error;
            this.Icon = new System.Drawing.Icon(this.GetType().Assembly.GetManifestResourceStream("Clamp.Explorer.Resources.Logo.ico"));
            this.lblTitle.Text = StringResources.FrmError_Titile;
            this.btnReset.BackgroundImage = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("Clamp.Explorer.Resources.Btnbg.png"));
            this.btnReset.Text = StringResources.FrmError_BtnReset;
            this.pbErrorImage.Image = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("Clamp.Explorer.Resources.FrmError.png"));
            this.pbClose.Image = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("Clamp.Explorer.Resources.FrmClose.png"));
            this.pbMinimum.Image = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("Clamp.Explorer.Resources.FrmMinimum.png"));

            this.pbErrorImage.Top = 132;
            this.pbErrorImage.Left = (this.Width - this.pbErrorImage.Width) / 2;

            this.lblError.Top = this.pbErrorImage.Top + this.pbErrorImage.Width + 32;
            this.lblError.Left = (this.Width - this.lblError.Width) / 2;
        }

        private void pbMinimum_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TopBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragOffset = this.PointToScreen(e.Location);
                dragOffset.X -= Location.X;
                dragOffset.Y -= Location.Y;
            }
        }

        private void TopBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point newLocation = this.PointToScreen(e.Location);

                newLocation.X -= dragOffset.X;
                newLocation.Y -= dragOffset.Y;

                Location = newLocation;
            }
        }

        public static void ShowFormError(string error)
        {
            Thread splashThread = new Thread(new ParameterizedThreadStart(arg =>
            {

                Application.Run(new FrmError(error));
            }));

            splashThread.IsBackground = true;
            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start(error);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
    }
    /// <summary>
    /// XP上焦聚会显示的不好，所以去掉
    /// </summary>
    public class NoFocusButton : Button
    {
        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
    }
}
