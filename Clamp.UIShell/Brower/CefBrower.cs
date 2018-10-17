using CefSharp;
using Clamp.UIShell.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Brower
{
    class CefBrower
    {
        public static bool CheckDependencies()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            var missingDeps = CefSharp.DependencyChecker.CheckDependencies(true, false, dir, dir, Path.Combine(dir, "CefSharp.BrowserSubprocess.exe"));

            if (missingDeps != null && missingDeps.Count > 0)
            {
                DebugHelper.WriteLine("Missing components:\r\n  " + string.Join("\r\n  ", missingDeps));

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
            settings.CefCommandLineArgs.Add("allow-running-insecure-content", "1");
            settings.MultiThreadedMessageLoop = multiThreadedMessageLoop;
            settings.LocalesDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "locales");
            settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CefSharp.BrowserSubprocess.exe");
            settings.ResourcesDirPath = AppDomain.CurrentDomain.BaseDirectory;
            settings.IgnoreCertificateErrors = SDShell.SDShellSettings.IgnoreCertificateErrors;

            if (osr)
            {
                settings.WindowlessRenderingEnabled = true;
                settings.CefCommandLineArgs.Add("disable-surfaces", "1");
                settings.CefCommandLineArgs.Add("disable-gpu", "1");
                settings.CefCommandLineArgs.Add("disable-web-security", "1");
            }

            settings.RegisterScheme(new CefCustomScheme
            {
                SchemeName = CefSharpSchemeHandlerFactory.SchemeName,
                SchemeHandlerFactory = new CefSharpSchemeHandlerFactory()
            });

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
