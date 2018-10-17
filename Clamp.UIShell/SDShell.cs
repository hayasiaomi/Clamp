using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Framework.InterProcess;
using Clamp.UIShell.Framework.Model;
using Clamp.UIShell.Brower;
using Clamp.UIShell.Framework;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using Clamp.Common;
using Clamp.UIShell.Framework.Services;

namespace Clamp.UIShell
{
    public class SDShell
    {
        private static List<WindowSimpleChromium> childrenChromiums = new List<WindowSimpleChromium>();
        private static ProcessWraper SDSuspenderProcess;
        private static ProcessWraper SDMessagerProcess;

        public static WindowChromium ChromiumWindow;

        public volatile static ProcessMonitor ProcessAdvices;

        public volatile static VirtualKeyboard VirtualKeyboard;

        public volatile static WinFormClientService WinFormClientService;

        public volatile static bool IsExiting = false;

        public static string FullVersion;

        /// <summary>
        /// 表示应用正在退出
        /// </summary>
        public static bool IsQuiting;

        public static DemandInfo Demand
        {
            get { return SDShellHelper.GetDemand(); }
        }

        /// <summary>
        /// 获得善点框的根路径
        /// </summary>
        /// <returns></returns>
        public static string RootPath
        {
            get { return SDShellHelper.GetRootPath(); }
        }

        /// <summary>
        /// 获得善点系统的根路径
        /// </summary>
        /// <returns></returns>
        public static string SDRootPath
        {
            get { return SDShellHelper.GetSDRootPath(); }
        }

        public static SDShellSettings SDShellSettings
        {
            get { return SDShellHelper.GetSDShellSettings(); }
        }

        /// <summary>
        /// 客户端需要的必要信息
        /// </summary>
        public static Dictionary<string, string> Essentials;

        /// <summary>
        /// 获得代理
        /// </summary>
        /// <returns></returns>
        public static ProxyInfo GetProxyInfo()
        {
            string proxyFile = System.IO.Path.Combine(SDShell.SDRootPath, "Proxy.json");

            if (File.Exists(proxyFile))
            {
                string proxyJson = File.ReadAllText(proxyFile, Encoding.UTF8);

                ProxyInfo proxyInfo = JsonConvert.DeserializeObject<ProxyInfo>(proxyJson);

                return proxyInfo;
            }

            return null;
        }

        /// <summary>
        /// 初始化配置项
        /// </summary>
        public static void Initialize()
        {
            SDShellHelper.GetSDShellSettings();
            SDShellHelper.GetDemand();
        }

        public static void AddChromium(WindowSimpleChromium windowSimpleChromium)
        {
            childrenChromiums.Add(windowSimpleChromium);
        }

        public static void RemoveChromium(WindowSimpleChromium windowSimpleChromium)
        {
            childrenChromiums.Remove(windowSimpleChromium);
        }

        public static void RemoveChromium(string url)
        {
            WindowSimpleChromium windowSimpleChromium = childrenChromiums.FirstOrDefault(cc => cc.Url == url);

            if (windowSimpleChromium != null)
                childrenChromiums.Remove(windowSimpleChromium);
        }

        public static void RemoveChromiumByIdentity(string identity)
        {
            WindowSimpleChromium windowSimpleChromium = childrenChromiums.FirstOrDefault(cc => cc.Identity == identity);

            if (windowSimpleChromium != null)
                childrenChromiums.Remove(windowSimpleChromium);
        }

        public static WindowSimpleChromium GetChromium(string url)
        {
            return childrenChromiums.FirstOrDefault(cc => cc.Url == url);
        }

        public static WindowSimpleChromium GetChromiumByIdentity(string identity)
        {
            return childrenChromiums.FirstOrDefault(cc => cc.Identity == identity);
        }

        /// <summary>
        /// 开始浮悬框
        /// </summary>
        public static void OpenSDSuspender()
        {
            if (SDShell.SDSuspenderProcess == null)
            {
                SDShell.SDSuspenderProcess = new ProcessWraper(System.IO.Path.Combine(SDShellHelper.GetRootPath(), "SDAssist.exe"), false);

                SDShell.SDSuspenderProcess.Open();
            }
        }


        /// <summary>
        /// 关闭浮悬框
        /// </summary>
        public static void ExitSDSuspender()
        {
            if (SDShell.SDSuspenderProcess != null)
            {
                SDShell.SDSuspenderProcess.Exit();
            }
        }

        /// <summary>
        /// 开始消息框
        /// </summary>
        public static void OpenSDMessager()
        {
            //if (SDShell.SDMessagerProcess == null)
            //{
            //    SDShell.SDMessagerProcess = new ProcessWraper(System.IO.Path.Combine(SDShellHelper.GetRootPath(), "SDMessager.exe"), false);

            //    SDShell.SDMessagerProcess.Open();
            //}
        }


        /// <summary>
        /// 关闭消息框
        /// </summary>
        public static void ExitSDMessager()
        {
            if (SDShell.SDMessagerProcess != null)
            {
                SDShell.SDMessagerProcess.Exit();
            }
        }


        /// <summary>
        /// 跳转默认页
        /// </summary>
        public static void RedirectAuthorize()
        {
            ChromiumWindow.Logout();
        }

        /// <summary>
        /// 退出
        /// </summary>
        public static void Exit()
        {
            CDBHelper.Modify("user_auth", "");

            if (SDSuspenderProcess != null)
                SDSuspenderProcess.Exit();

            while (childrenChromiums.Count > 0)
            {
                childrenChromiums[0].Close();
            }
        }

        public static void CreateChildrenChromium(string url, string title, int width, int height, double top, double left, Action<string> callback)
        {
            SDShell.ChromiumWindow.CreateChildrenChromium(url, title, width, height, left, top, callback);
        }

        public static void ShowVirtualKeyboard()
        {
            if (VirtualKeyboard == null)
            {
                VirtualKeyboard = new VirtualKeyboard();

                VirtualKeyboard.Top = SystemParameters.PrimaryScreenHeight - VirtualKeyboard.Height - 10;
                VirtualKeyboard.Left = SystemParameters.PrimaryScreenWidth - VirtualKeyboard.Width - 10;
            }

            VirtualKeyboard.Show();
        }

    }
}
