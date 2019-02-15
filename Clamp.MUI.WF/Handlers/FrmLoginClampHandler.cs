using Clamp.AppCenter.Handlers;
using Clamp.MUI.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.MUI.WF.Handlers
{
    public class FrmLoginClampHandler : MehtodClampHandler
    {
        private FrmLogin frmLogin;

        public FrmLoginClampHandler(FrmLogin frmLogin)
        {
            this.frmLogin = frmLogin;
        }

        public object Login(string username, string password, bool remember)
        {
            IAuthority authority = WFActivator.BundleContext.GetExtensionObjects<IAuthority>().FirstOrDefault();

            if (authority == null)
            {
                return "找不到授权模块";
            }

            AuthorityInfo authorityInfo = authority.GetAuthorityInfo();

            if (authorityInfo == null || authorityInfo.Username != username || authorityInfo.Password != password)
            {
                return "用户或密码不正确！";
            }

            //Thread mainThread = new Thread(new ThreadStart(() =>
            //{
            //    FrmMain frmMain = new FrmMain();

            //    frmMain.FrmLogin = this.frmLogin;

            //    Application.Run(frmMain);
            //}));

            //WFAppManager.Current.UIThreadStacks.Push(mainThread);

            //mainThread.SetApartmentState(ApartmentState.STA);
            //mainThread.Start();

            this.frmLogin.BeginInvoke(new Action(() =>
            {
                FrmMain frmMain = new FrmMain();

                frmMain.FrmLogin = this.frmLogin;

                frmMain.Show();

            }));

            return null;
        }
        public void Exit()
        {
            this.frmLogin.Invoke(new Action(() =>
            {
                if (MessageBox.Show(this.frmLogin, "确定退出?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    (WFAppManager.Current as WFAppManager).CurrentThread = null;

                    this.frmLogin.Close();
                }
            }));
        }
    }
}
