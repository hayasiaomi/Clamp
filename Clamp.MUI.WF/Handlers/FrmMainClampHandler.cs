using Clamp.AppCenter.Handlers;
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
    }
}
