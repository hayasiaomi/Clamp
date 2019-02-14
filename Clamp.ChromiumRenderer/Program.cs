using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.ChromiumRenderer
{
    class Program
    {
       
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            string assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            string localRuntimeDir = Path.Combine(assemblyDir, "FX");

            if (CfxRuntime.PlatformArch == CfxPlatformArch.x64)
                CfxRuntime.LibCefDirPath = Path.Combine(localRuntimeDir, "Cef64");
            else
                CfxRuntime.LibCefDirPath = Path.Combine(localRuntimeDir, "Cef");


            CfxRuntime.LibCfxDirPath = localRuntimeDir;

            int retval = CfxRuntime.ExecuteProcess();

            Environment.Exit(retval);
        }
    }
}
