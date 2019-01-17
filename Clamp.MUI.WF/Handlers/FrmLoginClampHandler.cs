using Clamp.AppCenter.Handlers;
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
            if ("00000" != username || "1234" != password)
            {
                return "用户或密码不正确！";
            }

            Thread mainThread = new Thread(new ThreadStart(() =>
            {
                FrmMain frmMain = new FrmMain();

                frmMain.FrmLogin = this.frmLogin;

                Application.Run(frmMain);
            }));

            (WFAppManager.Current as WFAppManager).CurrentThread = mainThread;

            mainThread.SetApartmentState(ApartmentState.STA);
            mainThread.Start();
         
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
