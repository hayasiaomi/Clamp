using CefSharp;
using Clamp.UIShell.Assist;
using Clamp.UIShell.Assist.Helpers;
using Microsoft.Win32;
using Clamp.UIShell.Assist;
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
using System.Windows;

namespace Clamp.UIShell.Assist.Brower
{
    /// <summary>
    /// 根浏览器交动的事件
    /// </summary>
    public class WinformBrowserObject
    {
        private WindowAdvices windowAdvices;

        public WinformBrowserObject(WindowAdvices windowAdvices)
        {
            this.windowAdvices = windowAdvices;
        }
        public void close()
        {
            this.windowAdvices.UnDisplay();
        }

        public void redirectSettings()
        {
            this.windowAdvices.RedirectSettings();
        }

        public void redirectHistory()
        {
            this.windowAdvices.RedirectHistory();
        }

        public void redirectDetails(string url)
        {
            this.windowAdvices.RedirectDetails(url);
        }
    }


}
