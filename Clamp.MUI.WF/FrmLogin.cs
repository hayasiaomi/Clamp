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

        public override IClampHandler GetClampHandler()
        {
            return new FrmLoginClampHandler(this);
        }
    }
}
