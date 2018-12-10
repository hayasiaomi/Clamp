using Clamp.MUI.Biz;
using Clamp.MUI.Framework.INI;
using Clamp.MUI.Framework.UI;
using Clamp.MUI.Helpers;
using Clamp.MUI.Splash;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Clamp.MUI
{
    internal class ChromiumSettings
    {
        private volatile static Dictionary<string, object> settings = new Dictionary<string, object>();

        internal static List<FrmChromium> ChildrenChromiums = new List<FrmChromium>();

        /// <summary>
        /// 主框
        /// </summary>
        internal volatile static FrmMainChromium FrmMainChromium;

        internal volatile static Thread SplashUIThread;

        internal volatile static SplashResult SplashResult;

        internal static bool IsChromiumInitialized;

        internal static string AuthorizeUrl = "embedded://Clamp.Explorer.WebSites/default.html";

        internal static ChildProcess ChildProcess;

        internal volatile static bool IsExiting = false;

        /// <summary>
        /// 是否为主机
        /// </summary>
        internal static bool IsMainFrame;

        /// <summary>
        /// 主页的URL
        /// </summary>
        internal static string InitializeUrl;


        /// <summary>
        /// 终端ID
        /// </summary>
        internal static string PCID;

        /// <summary>
        /// 店门ID
        /// </summary>
        internal static string OrgExtCode;

        /// <summary>
        /// 终端版本号
        /// </summary>
        internal static string FinalVersion;

        /// <summary>
        /// 服务地址
        /// </summary>
        internal static string ServerAddress = "127.0.0.1";

        /// <summary>
        /// 通信端口
        /// </summary>
        internal static int Port = 31234;


        /// <summary>
        /// 本地文化
        /// </summary>
        internal static string CultureName;


        /// <summary>
        /// 
        /// </summary>
        internal static bool IsWinformService;

        /// <summary>
        /// 是否为Debug模式
        /// </summary>
        internal static bool Debug;




        /// <summary>
        /// 客户端需要的必要信息
        /// </summary>
        internal static Dictionary<string, string> Essentials;


        /// <summary>
        /// 初始化配置项
        /// </summary>
        internal static void InitializeClampSettings()
        {
            INIManager iniManager = new INIManager();

            iniManager.Initialize();

            InitializeUrl = iniManager.Get("InitializeUrl");
            CultureName = iniManager.Get("CultureName");
            IsWinformService = iniManager.GetBoolean("IsWinformService");

            if (string.IsNullOrWhiteSpace(CultureName))
                CultureName = "zh-CN";

            string sDebug = iniManager.Get("SDebug");

            if (!string.IsNullOrWhiteSpace(sDebug) && Convert.ToBoolean(sDebug))
                Debug = true;
            else
                Debug = false;
        }

        /// <summary>
        /// 初始化必备的数据
        /// </summary>
        internal static void InitializeEssentials()
        {
            Essentials = EssentialHelper.GetAll();

            IsMainFrame = Essentials["InstallMode"] == "Mainframe";
            ServerAddress = Essentials["ServerIp"];

            if (Environment.Is64BitOperatingSystem)
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian"))
                {
                    PCID = Convert.ToString(shanDianRegistry.GetValue("PCID"));
                }
            }
            else
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian"))
                {
                    PCID = Convert.ToString(shanDianRegistry.GetValue("PCID"));
                }
            }

            if (string.IsNullOrWhiteSpace(PCID))
                PCID = Essentials["PCID"];
        }


    }
}
