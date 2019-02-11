using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using Clamp.AppCenter;
using Clamp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace Clamp.MUI
{
    class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (IClampBundle clampBundle = ClampBundleFactory.GetClampBundle())
            {
                clampBundle.Start();

                IAppManager appManager = clampBundle.GetExtensionObjects<IAppManager>()?.FirstOrDefault();

                if (appManager != null)
                {
                    appManager.Initialize();

                    appManager.Run();
                }

                //clampBundle.Stop();

                //clampBundle.WaitForStop();
            }
        }
    }
}
