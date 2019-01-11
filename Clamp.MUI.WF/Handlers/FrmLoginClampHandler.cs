using Clamp.AppCenter.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public object Login(string username, string password)
        {
            frmLogin.Invoke(new Action(() =>
            {
                MessageBox.Show("aaa");
            }));
            return null;
        }
    }
}
