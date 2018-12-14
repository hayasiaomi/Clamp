using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.Explorer.ChromiumRenderer
{
    class Program
    {
       
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (CfxRuntime.PlatformArch == CfxPlatformArch.x64)
                CfxRuntime.LibCefDirPath = @"cef64";
            else
                CfxRuntime.LibCefDirPath = @"cef";

            int retval = CfxRuntime.ExecuteProcess();

            Environment.Exit(retval);
        }
    }
}
