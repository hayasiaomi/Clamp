using Clamp.UIShell.Framework.Shortcut;
using Clamp.UIShell.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using Clamp.UIShell.Brower;
using Microsoft.Win32;
using System.Security.Principal;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Framework;
using System.Windows;
using Clamp.UIShell.Win32;
using Clamp.UIShell.Forms;
using CefSharp;
using Clamp.UIShell.Framework.Services;
using Clamp.UIShell.Framework.Model;

namespace Clamp.UIShell
{
    static class Program
    {
        /// <summary>
        /// 应用于唯一的实列应用
        /// </summary>
        private static Mutex mutex = new Mutex(true, "A8C6520F-A51F-4C7C-8A2C-DF974B6AA6E0");

        [STAThread]
        public static void Main(string[] args)
        {
            if (mutex.WaitOne(TimeSpan.FromSeconds(2), true))
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Environment.CurrentDirectory = Path.GetDirectoryName(new System.Uri(typeof(Program).Assembly.CodeBase).LocalPath);

                System.Windows.Forms.Application.EnableVisualStyles();
                System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

                Program.Run();

                mutex.ReleaseMutex();
            }
            else
            {
                Process[] shanDianProcesses = Process.GetProcessesByName("SDShell");

                Process mainProcess = shanDianProcesses.FirstOrDefault(sdp => sdp.Id != Process.GetCurrentProcess().Id);

                if (mainProcess != null)
                {
                    IntPtr windowPtr = mainProcess.Handle;

                    if (windowPtr != IntPtr.Zero)
                    {
                        WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
                        User32Dll.SetForegroundWindow(windowPtr);

                        User32Dll.GetWindowPlacement(windowPtr, ref placement);

                        if (placement.showCmd == 2)
                        {
                            User32Dll.ShowWindow(windowPtr, WINOWSHOWSTYLE.ShowNormal);
                        }

                        User32Dll.PostMessage((IntPtr)NativeConstant.HWND_BROADCAST, NativeConstant.WM_SHOWME, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
        }

        /// <summary>
        /// 执行程序
        /// </summary>
        internal static void Run()
        {
            try
            {
                Program.RunApplication();
            }
            catch (Exception ex)
            {
                NLogService.Error("应用出错了", ex);

                System.Windows.Forms.MessageBox.Show(ex.ToString(), SDResources.Alert_Error_Title);

                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// 处理没有处理过的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            NLogService.Error("应用出错了", (e.ExceptionObject as Exception));

            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// 运行应用
        /// </summary>
        internal static void RunApplication()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            FrmSplash.ShowSplashScreen();
            System.Windows.Forms.Application.DoEvents();

            int interval = 200;

            FrmSplash.SetProgressRate(5);
            Thread.Sleep(interval);

            NLogService.Info("开始清除浏览缓存");
            ClearChromiumCache();
            NLogService.Info("结束清除浏览缓存");

            FrmSplash.SetProgressRate(10);
            Thread.Sleep(interval);

            NLogService.Info("开始初始化配置");
            SDShell.Initialize();
            NLogService.Info("结束初始化配置");

            FrmSplash.SetProgressRate(25);
            Thread.Sleep(interval);

            NLogService.Info("开始初始化善点的后台服务");

#if !DEVELOP
            if (!InstallService.OpenShanDianServices())
            {
                FrmSplash.CloseForm();
                FrmError.ShowErrorScreen(SDResources.CheckExecute_ServerError);
                return;
            }
#endif

            NLogService.Info("结束初始化善点的后台服务");

            FrmSplash.SetProgressRate(35);
            Thread.Sleep(interval);

            NLogService.Info("开始获得善点系统的信息");
            SystemInfo systemInfo = SDService.GetSystemInfo();

            if (systemInfo == null)
            {
                FrmSplash.CloseForm();
                FrmError.ShowErrorScreen(SDResources.CheckExecute_ErrorNetworkService);
                return;
            }

            SDShell.FullVersion = systemInfo.Version;

            NLogService.Info("结束获得善点系统的信息");

            FrmSplash.SetProgressRate(45);
            Thread.Sleep(interval);

            NLogService.Info("开始初始化UI环境");

            Cef.EnableHighDPISupport();

            CefBrower.Init(true, true);

            System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
            ComponentDispatcher.ThreadIdle -= ComponentDispatcher_ThreadIdle;
            ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;

            App app = new App();
            app.Exit += App_Exit;
            app.InitializeComponent();
            app.ShutdownMode = ShutdownMode.OnLastWindowClose;

            FrmSplash.SetProgressRate(100);
            Thread.Sleep(interval);
            NLogService.Info("结束初始化UI环境");

            if (!systemInfo.IsActivited)
            {
                WindowActivited windowActivited = new WindowActivited();
                app.MainWindow = windowActivited;
                app.Run(windowActivited);
            }
            else
            {
                WindowAuthority windowAuthority = new WindowAuthority();
                app.MainWindow = windowAuthority;
                app.Run(windowAuthority);
            }

            CefSharp.Cef.Shutdown();
        }

        private static void App_Exit(object sender, ExitEventArgs e)
        {
            CefSharp.Cef.Shutdown();
        }


        private static void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.RaiseIdle(e);
        }

        /// <summary>
        /// 删除浏览器的缓存
        /// </summary>
        internal static void ClearChromiumCache()
        {
            try
            {
                string cacheDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

                if (Directory.Exists(cacheDirectory))
                {
                    Directory.Delete(cacheDirectory, true);
                }

                string cookiesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies");

                if (Directory.Exists(cookiesDirectory))
                {
                    Directory.Delete(cookiesDirectory, true);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
