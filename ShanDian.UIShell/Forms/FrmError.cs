using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace ShanDian.UIShell.Forms
{
    public partial class FrmError : Form
    {
        private static FrmError mFrmError = null;
        private static Thread mThread = null;


        private Point dragOffset = new Point();

        public string Error { private set; get; }

        private FrmError(string error)
        {
            this.Error = error;

            InitializeComponent();
        }

        private void FrmError_Shown(object sender, EventArgs e)
        {
            this.lblError.Text = this.Error;
            this.Icon = new System.Drawing.Icon(this.GetType().Assembly.GetManifestResourceStream("ShanDian.UIShell.Resources.Logo.ico"));
            this.lblTitle.Text = SDResources.FrmError_Titile + ServerHelper.GetServerVersion();
            this.btnReset.BackgroundImage = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("ShanDian.UIShell.Resources.Btnbg.png"));
            this.btnReset.Text = SDResources.FrmError_BtnReset;
            this.pbErrorImage.Image = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("ShanDian.UIShell.Resources.FrmError.png"));
            this.pbClose.Image = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("ShanDian.UIShell.Resources.FrmClose.png"));
            this.pbMinimum.Image = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("ShanDian.UIShell.Resources.FrmMinimum.png"));

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        public static void ShowErrorScreen(string error)
        {
            if (mFrmError != null)
                return;

            mThread = new Thread(new ParameterizedThreadStart(FrmError.ShowForm));
            mThread.IsBackground = true;
            mThread.SetApartmentState(ApartmentState.STA);
            mThread.Start(error);

            while (mFrmError == null || mFrmError.IsHandleCreated == false)
            {
                System.Threading.Thread.Sleep(50);
            }

            mThread.Join();
        }

        private static void ShowForm(object state)
        {
            mFrmError = new FrmError(Convert.ToString(state));
            Application.Run(mFrmError);
        }

        private void FrmError_Load(object sender, EventArgs e)
        {
            this.Activate();
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
