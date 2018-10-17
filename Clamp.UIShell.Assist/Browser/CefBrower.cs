using CefSharp;
using Clamp.UIShell.Assist.Helpers;
using Clamp.UIShell.Framework.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Assist.Brower
{
    class CefBrower
    {
        public static bool CheckDependencies()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            var missingDeps = CefSharp.DependencyChecker.CheckDependencies(true, false, dir, dir, Path.Combine(dir, "HydraCefRender.exe"));

            if (missingDeps != null && missingDeps.Count > 0)
            {
                NLogService.Info("Missing components:\r\n  " + string.Join("\r\n  ", missingDeps));

                return false;
            }

            return true;
        }


        public static void Init(bool osr, bool multiThreadedMessageLoop)
        {
            var settings = new CefSettings();


            settings.UserAgent = "CefSharp Browser" + Cef.CefSharpVersion;
            settings.CachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            settings.CefCommandLineArgs.Add("js-flags", "--harmony_proxies --harmony_collections");
            settings.MultiThreadedMessageLoop = multiThreadedMessageLoop;
            settings.LocalesDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "locales");
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CefSharp.BrowserSubprocess.exe");
            settings.ResourcesDirPath = AppDomain.CurrentDomain.BaseDirectory;

            if (osr)
            {
                settings.WindowlessRenderingEnabled = true;
                settings.CefCommandLineArgs.Add("disable-surfaces", "1");
                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                settings.CefCommandLineArgs.Add("disable-web-security", "1");
            }

            settings.CefCommandLineArgs.Add("no-proxy-server", "1");

            settings.FocusedNodeChangedEnabled = true;

            Cef.OnContextInitialized = delegate
            {
                var cookieManager = Cef.GetGlobalCookieManager();
                cookieManager.SetStoragePath("cookies", true);
                cookieManager.SetSupportedSchemes("custom");

                using (var context = Cef.GetGlobalRequestContext())
                {
                    string errorMessage;

                    context.SetPreference("webkit.webprefs.plugins_enabled", true, out errorMessage);
                }
            };

            if (!Cef.Initialize(settings, true, !Debugger.IsAttached))
            {
                throw new Exception("Unable to Initialize Cef");
            }
        }
    }
}
