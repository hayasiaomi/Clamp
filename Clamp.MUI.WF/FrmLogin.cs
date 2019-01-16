using Chromium.Event;
using Clamp.AppCenter;
using Clamp.AppCenter.Handlers;
using Clamp.MUI.WF.Controls;
using Clamp.MUI.WF.Handlers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Clamp.MUI.WF
{
    public partial class FrmLogin : FrmBase
    {

        internal FrmMain FrmMain { set; get; }

        public FrmLogin()
        {
            InitializeComponent();

            this.Load += FrmLogin_Load;
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            this.Load -= FrmLogin_Load;

            if (AppManager.Current.ClampConfs.ContainsKey(AppCenterConstant.CFX_LOGIN_URL))
            {
                this.LoadUrl(AppManager.Current.ClampConfs[AppCenterConstant.CFX_LOGIN_URL]);
            }
            else
            {
                this.LoadUrl("about:blank");
            }
        }

        protected override void OnChromiumLoadEnd(object sender, CfxOnLoadEndEventArgs e)
        {
            //if (this.FrmMain != null && !this.FrmMain.IsDisposed)
            //{
            //    this.FrmMain.Invoke(new Action(() =>
            //    {
            //        this.FrmMain.Close();
            //        this.FrmMain.Dispose();
            //    }));
            //}
        }

        public override IClampHandler GetClampHandler()
        {
            return new FrmLoginClampHandler(this);
        }
    }
}
