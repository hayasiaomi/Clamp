using Chromium;
using Chromium.Event;
using Clamp.AppCenter;
using Clamp.AppCenter.Handlers;
using Clamp.MUI.WF.CFX;
using Clamp.MUI.WF.Controls;
using Clamp.MUI.WF.Handlers;
using Clamp.MUI.WF.Windows;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.MUI.WF
{
    public partial class FrmMain : FrmBase
    {
        internal FrmLogin FrmLogin { set; get; }

        public FrmMain()
        {
            InitializeComponent();

            this.Load += FrmMain_Load;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.Load -= FrmMain_Load;

            if (WFAppManager.Current.ConfigYEUXMaps.ContainsKey(AppCenterConstant.CFX_INIT_URL))
            {
                this.LoadUrl(WFAppManager.Current.ConfigYEUXMaps[AppCenterConstant.CFX_INIT_URL]);
            }
            else
            {
                this.LoadUrl("about:blank");
            }
        }

        public override IClampHandler GetClampHandler()
        {
            return new FrmMainClampHandler(this);
        }

        protected override void OnChromiumLoadEnd(object sender, CfxOnLoadEndEventArgs e)
        {
            if (this.FrmLogin != null && !this.FrmLogin.IsDisposed)
            {
                this.FrmLogin.Invoke(new Action(() =>
                {
                    this.FrmLogin.Close();
                    this.FrmLogin.Dispose();
                }));
            }
        }

    }
}
