using CefSharp;
using ShanDian.UIShell.Assist;
using ShanDian.UIShell.Assist.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShanDian.UIShell.Assist.Brower
{
    /// <summary>
    /// 根浏览器交动的事件
    /// </summary>
    public class WinformBrowserObject
    {
        private FrmMain frmMain;

        public WinformBrowserObject(FrmMain frmMain)
        {
            this.frmMain = frmMain;
        }
        public void close()
        {
            this.frmMain.UnDisplay();
        }

        public void redirectSettings()
        {
            this.frmMain.RedirectSettings();
        }

        public void redirectHistory()
        {
            this.frmMain.RedirectHistory();
        }

        public void redirectDetails(string url)
        {
            this.frmMain.RedirectDetails(url);
        }
    }


}
