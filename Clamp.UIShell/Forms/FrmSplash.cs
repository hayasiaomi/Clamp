using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace Clamp.UIShell.Forms
{
    // The SplashScreen class definition.  AKO Form
    public partial class FrmSplash : Form
    {
        private static FrmSplash mFrmSplash = null;
        private static Thread mThread = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public FrmSplash()
        {
            InitializeComponent();
        }

        public void KillMe(object o, EventArgs e)
        {
            this.Close();
            this.Cursor = Cursors.Default; ;
        }


        #region Public Static Methods

        static public void ShowSplashScreen()
        {
            if (mFrmSplash != null)
                return;
            mThread = new Thread(new ThreadStart(FrmSplash.ShowForm));
            mThread.IsBackground = true;
            mThread.SetApartmentState(ApartmentState.STA);
            mThread.Start();

            while (mFrmSplash == null || mFrmSplash.IsHandleCreated == false)
            {
                System.Threading.Thread.Sleep(50);
            }
        }

        static public void CloseForm()
        {
            if (mFrmSplash == null)
                return;

            mFrmSplash.Invoke(new EventHandler(mFrmSplash.KillMe));
            mFrmSplash.Dispose();
            mFrmSplash = null;
        }

        static public void SetProgressRate(int rate)
        {
            if (mFrmSplash == null)
                return;
            mFrmSplash.BeginInvoke(new Action(() =>
            {
                mFrmSplash.pbarInitalized.Value = rate;
            }));
        }


        #endregion Public Static Methods

        static private void ShowForm()
        {
            mFrmSplash = new FrmSplash();
            Application.Run(mFrmSplash);
        }

        public static FrmSplash GetSplashScreen()
        {
            return mFrmSplash;
        }

    }
}

