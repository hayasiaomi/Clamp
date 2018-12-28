using Chromium;
using Chromium.Event;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using Clamp.AppCenter;
using Clamp.AppCenter.CFX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Clamp.MUI.WPF.UI
{
    public class UILauncher
    {
        public const string CURRENT_CEF_VERSION = "3.2623.1401";

        internal static string ClampLibCefDirPath = null;
        internal static string ClampLocalesDir = null;
        internal static string ClampBrowserSubprocessPath = "YEYECefRenderer.exe";
        internal static readonly string ApplicationDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        internal static readonly RuntimeArch PlatformArch = CfxRuntime.PlatformArch == CfxPlatformArch.x64 ? RuntimeArch.x64 : RuntimeArch.x86;

        internal static List<GCHandle> SchemeHandlerGCHandles = new List<GCHandle>();

        /// <summary>
        /// 初始化浏览器
        /// </summary>
        /// <param name="localRuntimeDir"></param>
        /// <param name="BeforeChromiumInitialize"></param>
        /// <param name="BeforeCommandLineProcessing"></param>
        /// <returns></returns>
        public static bool InitializeChromium(string localRuntimeDir = null, Action<OnBeforeCfxInitializeEventArgs> BeforeChromiumInitialize = null, Action<CfxOnBeforeCommandLineProcessingEventArgs> BeforeCommandLineProcessing = null)
        {
            if (PrepareRuntime(localRuntimeDir))
            {
                ChromiumWebBrowser.OnBeforeCfxInitialize += (e) =>
                {
                    //if (!Directory.Exists(cachePath))
                    //    Directory.CreateDirectory(cachePath);

                    e.Settings.LocalesDirPath = ClampLocalesDir;
                    e.Settings.ResourcesDirPath = ClampLibCefDirPath;
                    e.Settings.Locale = "zh-CN";
                    e.Settings.CachePath = Path.Combine(ApplicationDataDir, typeof(UILauncher).Assembly.GetName().Name, "Cache");
                    e.Settings.LogSeverity = CfxLogSeverity.Disable;
                    e.Settings.BrowserSubprocessPath = Path.Combine(localRuntimeDir, ClampBrowserSubprocessPath);
                    e.Settings.NoSandbox = true;
                    //e.Settings.RemoteDebuggingPort = 8888;
                    //e.Settings.IgnoreCertificateErrors = true;
                    e.Settings.MultiThreadedMessageLoop = true;
                    e.Settings.WindowlessRenderingEnabled = true;

                    BeforeChromiumInitialize?.Invoke(e);
                };

                ChromiumWebBrowser.OnBeforeCommandLineProcessing += (args) =>
                {
                    Console.WriteLine("处理命令行参数。。。");

                    args.CommandLine.AppendSwitchWithValue("--no-proxy-server", "1");
                    args.CommandLine.AppendSwitchWithValue("--disable-local-storage", "1");
                    args.CommandLine.AppendSwitchWithValue("--disable-application-cache", "1");
                    args.CommandLine.AppendSwitchWithValue("--disable-gpu-program-cache", "1");
                    args.CommandLine.AppendSwitchWithValue("--disable-cache", "1");
                    args.CommandLine.AppendSwitchWithValue("--disable-cache", "1");
                    args.CommandLine.AppendSwitchWithValue("--disable-surfaces", "1");
                    args.CommandLine.AppendSwitchWithValue("--disable-web-security", "1");

                    BeforeCommandLineProcessing?.Invoke(args);

                    Console.WriteLine(args.CommandLine.CommandLineString);
                };

                ChromiumWebBrowser.OnRegisterCustomSchemes += args =>
                {
                    args.Registrar.AddCustomScheme("embedded", false, false, false);
                };

                try
                {
                    ChromiumWebBrowser.Initialize();

                    RegisterLocalScheme();

                    RegisterClampScheme();

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine(ex.InnerException);
                }
            }
            return false;
        }

        private static void RegisterClampScheme()
        {
            ClampSchemeHandlerFactory scheme = new ClampSchemeHandlerFactory("http");
            GCHandle gchandle = GCHandle.Alloc(scheme);

            SchemeHandlerGCHandles.Add(gchandle);

            RegisterScheme("http", null, scheme);
        }


        private static void RegisterLocalScheme()
        {
            LocalSchemeHandlerFactory scheme = new LocalSchemeHandlerFactory();
            GCHandle gchandle = GCHandle.Alloc(scheme);

            SchemeHandlerGCHandles.Add(gchandle);

            RegisterScheme("local", null, scheme);
        }

        public static void RegisterEmbeddedScheme(Assembly assembly, string schemeName = "http", string domainName = null)
        {
            if (string.IsNullOrEmpty(schemeName))
            {
                throw new ArgumentNullException("schemeName", "必须为scheme指定名称。");
            }

            EmbeddedSchemeHandlerFactory embedded = new EmbeddedSchemeHandlerFactory(schemeName, domainName, assembly);
            GCHandle gchandle = GCHandle.Alloc(embedded);

            SchemeHandlerGCHandles.Add(gchandle);

            RegisterScheme(embedded.SchemeName, domainName, embedded);
        }

        public static void RegisterScheme(string schemeName, string domain, CfxSchemeHandlerFactory factory)
        {
            if (string.IsNullOrEmpty(schemeName))
            {
                throw new ArgumentNullException("schemeName", "必须为scheme指定名称。");
            }

            CfxRuntime.RegisterSchemeHandlerFactory(schemeName, domain, factory);
        }

        /// <summary>
        /// 检测运行的环境
        /// </summary>
        /// <param name="localRuntimeDir"></param>
        /// <returns></returns>
        public static bool PrepareRuntime(string localRuntimeDir = null)
        {
            if (IsLocalRuntimeExisits(localRuntimeDir) == false)
            {
                //MessageBox.Show($"CEF Runtime is not found.\r\nCEF Runtime should be in\r\n\"{System.IO.Path.Combine(Application.StartupPath, "fx\\")}\"", "CEF Runtime initialize faild", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            CfxRuntime.LibCefDirPath = ClampLibCefDirPath;

            //Application.Idle += Application_Idle;
            Application.Current.Exit += Application_ApplicationExit;

            return true;
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            foreach (var handle in SchemeHandlerGCHandles)
            {
                handle.Free();
            }

            CfxRuntime.Shutdown();
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            CfxRuntime.DoMessageLoopWork();
        }

        private static bool IsLocalRuntimeExisits(string localRuntimeDir = null)
        {
            if (string.IsNullOrWhiteSpace(localRuntimeDir))
                localRuntimeDir = AppDomain.CurrentDomain.BaseDirectory;

            var libCfxDllName = "libcfx.dll";
            var libcfxLibName = "libcfx.lib";
            var libcfxExpName = "libcfx.exp";
            var libcfxIlkName = "libcfx.ilk";

            if (PlatformArch == RuntimeArch.x64)
            {
                libCfxDllName = "libcfx64.dll";
                libcfxLibName = "libcfx64.lib";
                libcfxExpName = "libcfx64.exp";
                libcfxIlkName = "libcfx64.ilk";
            }


            if (PlatformArch == RuntimeArch.x64)
                ClampLibCefDirPath = Path.Combine(localRuntimeDir, @"Cef64");
            else
                ClampLibCefDirPath = Path.Combine(localRuntimeDir, @"Cef");

            ClampLocalesDir = Path.Combine(localRuntimeDir, ClampLibCefDirPath, @"locales");

            var cfxDllFile = Path.Combine(ClampLibCefDirPath, libCfxDllName);

            Dictionary<string, bool> doubtsDetectResults = new Dictionary<string, bool>()
            {
                [libCfxDllName] = File.Exists(Path.Combine(localRuntimeDir, libCfxDllName)),
                [libcfxLibName] = File.Exists(Path.Combine(localRuntimeDir, libcfxLibName)),
                [libcfxIlkName] = File.Exists(Path.Combine(localRuntimeDir, libcfxIlkName)),
                [libcfxExpName] = File.Exists(Path.Combine(localRuntimeDir, libcfxExpName)),
                ["en-US.pak"] = File.Exists(Path.Combine(ClampLocalesDir, "en-US.pak")),
                ["cef.pak"] = File.Exists(Path.Combine(ClampLibCefDirPath, "cef.pak")),
                ["cef_sandbox.lib"] = File.Exists(Path.Combine(ClampLibCefDirPath, "cef_sandbox.lib")),
                ["cef_100_percent.pak"] = File.Exists(Path.Combine(ClampLibCefDirPath, "cef_100_percent.pak")),
                ["cef_200_percent.pak"] = File.Exists(Path.Combine(ClampLibCefDirPath, "cef_200_percent.pak")),
                ["cef_extensions.pak"] = File.Exists(Path.Combine(ClampLibCefDirPath, "cef_extensions.pak")),
                ["chrome_elf.dll"] = File.Exists(Path.Combine(ClampLibCefDirPath, "d3dcompiler_43.dll")),
                ["d3dcompiler_43.dll"] = File.Exists(Path.Combine(ClampLibCefDirPath, "d3dcompiler_43.dll")),
                ["d3dcompiler_47.dll"] = File.Exists(Path.Combine(ClampLibCefDirPath, "d3dcompiler_47.dll")),
                ["devtools_resources.pak"] = File.Exists(Path.Combine(ClampLibCefDirPath, "devtools_resources.pak")),
                ["icudtl.dat"] = File.Exists(Path.Combine(ClampLibCefDirPath, "icudtl.dat")),
                ["libcef.dll"] = File.Exists(Path.Combine(ClampLibCefDirPath, "libcef.dll")),
                ["libcef.lib"] = File.Exists(Path.Combine(ClampLibCefDirPath, "libcef.lib")),
                ["libEGL.dll"] = File.Exists(Path.Combine(ClampLibCefDirPath, "libEGL.dll")),
                ["libGLESv2.dll"] = File.Exists(Path.Combine(ClampLibCefDirPath, "libGLESv2.dll")),
                ["natives_blob.bin"] = File.Exists(Path.Combine(ClampLibCefDirPath, "natives_blob.bin")),
                ["snapshot_blob.bin"] = File.Exists(Path.Combine(ClampLibCefDirPath, "snapshot_blob.bin")),
                //["v8_context_snapshot.bin"] = File.Exists(Path.Combine(DoubtsLibCefDirPath, "v8_context_snapshot.bin")),
                ["widevinecdmadapter.dll"] = File.Exists(Path.Combine(ClampLibCefDirPath, "widevinecdmadapter.dll"))
            };

            return doubtsDetectResults.Count(p => p.Value == true) == doubtsDetectResults.Count;

        }

    }
}
