﻿using Clamp.AppCenter.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Clamp.MUI.WF.Handlers
{
    public class FrmMainClampHandler : MehtodClampHandler
    {
        private FrmMain frmMain;

        public FrmMainClampHandler(FrmMain frmMain)
        {
            this.frmMain = frmMain;
        }

        public object Logout(string username, string password, bool remember)
        {

            Thread mainThread = new Thread(new ThreadStart(() =>
            {
                FrmLogin frmLogin = new FrmLogin();

                frmLogin.FrmMain = this.frmMain;

                Application.Run(frmLogin);
            }));

            mainThread.SetApartmentState(ApartmentState.STA);
            mainThread.Start();

            (WFAppManager.Current as WFAppManager).CurrentThread = mainThread;

            return null;
        }
    }
}
