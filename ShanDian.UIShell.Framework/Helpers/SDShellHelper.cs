using Microsoft.Win32;
using Newtonsoft.Json;
using ShanDian.Common.HTTP;
using ShanDian.Common.Initial;
using ShanDian.UIShell.Framework.Model;
using ShanDian.UIShell.Framework.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ShanDian.UIShell.Framework.Helpers
{
    public class SDShellHelper
    {
        private static SDShellSettings UIShellSettingsCache;
        private static DemandInfo demandCache;

     

        /// <summary>
        /// 装载激活的凭证信息，放到内存
        /// </summary>
        /// <returns></returns>
        public static void SetupDemandMemory(DemandInfo demand)
        {
            if (demand != null)
            {
                demandCache = demand;
            }
        }

        public static int GetMainListener()
        {
            return GetSDShellSettings().MainListener;
        }


        /// <summary>
        /// 获得激活的凭证信息
        /// </summary>
        /// <returns></returns>
        public static DemandInfo GetDemand()
        {
            if (demandCache == null)
            {
                demandCache = SDService.GetDemandInfo();
            }

            return demandCache;
        }

        /// <summary>
        /// 获得注册表的GUID。
        /// </summary>
        /// <returns></returns>
        public static string GetRegistryPCID()
        {
            RegistryKey localKey32;
            RegistryKey shanDianRegistry;

            if (Environment.Is64BitOperatingSystem)
            {
                localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian", true);
            }
            else
            {
                localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian", true);
            }

            if (shanDianRegistry != null)
            {
                object pcidValue = shanDianRegistry.GetValue("PCID");

                if (pcidValue != null)
                    return System.Convert.ToString(pcidValue);
            }

            if (localKey32 != null)
                localKey32.Dispose();

            if (shanDianRegistry != null)
                shanDianRegistry.Dispose();

            return string.Empty;
        }

        /// <summary>
        /// 保存PICD到注册表
        /// </summary>
        /// <param name="pcid"></param>
        public static void SaveRegistryPCID(string pcid)
        {
            RegistryKey localKey32;
            RegistryKey shanDianRegistry;

            if (Environment.Is64BitOperatingSystem)
            {
                localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian", true);
            }
            else
            {
                localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian", true);
            }

            if (shanDianRegistry == null)
            {
                shanDianRegistry = localKey32.CreateSubKey(@"SOFTWARE\ShanDian");

                shanDianRegistry.SetValue("PCID", pcid);
            }
            else
            {
                object pcidValue = shanDianRegistry.GetValue("PCID");

                if (pcidValue == null || Convert.ToString(pcidValue) != pcid)
                {
                    shanDianRegistry.SetValue("PCID", pcid);
                }
            }

            if (localKey32 != null)
                localKey32.Dispose();

            if (shanDianRegistry != null)
                shanDianRegistry.Dispose();
        }

        /// <summary>
        /// 保存门店编号
        /// </summary>
        /// <param name="pcid"></param>
        public static void SaveMerchantNo(string merchantNo)
        {
            RegistryKey localKey32;
            RegistryKey shanDianRegistry;

            if (Environment.Is64BitOperatingSystem)
            {
                localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian", true);
            }
            else
            {
                localKey32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\ShanDian", true);
            }

            if (shanDianRegistry == null)
            {
                shanDianRegistry = localKey32.CreateSubKey(@"SOFTWARE\ShanDian");

                shanDianRegistry.SetValue("MerchantNo", merchantNo);
            }
            else
            {
                object merchantNoValue = shanDianRegistry.GetValue("MerchantNo");

                if (merchantNoValue == null || Convert.ToString(merchantNoValue) != merchantNo)
                {
                    shanDianRegistry.SetValue("MerchantNo", merchantNo);
                }
            }

            if (localKey32 != null)
                localKey32.Dispose();

            if (shanDianRegistry != null)
                shanDianRegistry.Dispose();
        }

        /// <summary>
        /// 获得应用的根目录
        /// </summary>
        /// <returns></returns>
        public static string GetRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }


        /// <summary>
        /// 获得善点的根目录
        /// </summary>
        /// <returns></returns>
        public static string GetSDRootPath()
        {
            return Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
        }


        public static string GetSDHost(string module, string fragment)
        {
            return $"http://{UIShellSettingsCache.ShanDianHost}:{UIShellSettingsCache.ShanDianPort}/{module}/api/1.0.0.0/{fragment}";
        }

        public static string GetSelfHost(string fragment)
        {
            return $"Http://127.0.0.1:{GetSDShellSettings().MainListener}/sd{fragment}";
        }

        /// <summary>
        /// 获得激活的凭证信息
        /// </summary>
        /// <returns></returns>
        public static SDShellSettings GetSDShellSettings()
        {
            if (UIShellSettingsCache == null)
            {
                UIShellSettingsCache = new SDShellSettings();

                string UISSFile = Path.Combine(SDShellHelper.GetRootPath(), "sdshell.ini");

                FileInfo UISSFileInfo = new FileInfo(UISSFile);

                InitialFile initials = InitialFile.LoadFromFile(UISSFile);

                if (UISSFileInfo.Exists && UISSFileInfo.Length > 0)
                {
                    UIShellSettingsCache.MainListener = initials["MainListener"].IntValue;
                    UIShellSettingsCache.InitializeUrl = initials["InitializeUrl"].StringValueTrimmed;
                    UIShellSettingsCache.AdvicesUrl = initials["AdvicesUrl"].StringValueTrimmed;
                    UIShellSettingsCache.CultureName = initials["CultureName"].StringValueTrimmed;
                    UIShellSettingsCache.ShowDevTools = initials["ShowDevTools"].BoolValue;
                    UIShellSettingsCache.PrintProcess = initials["PrintProcess"].BoolValue;
                    UIShellSettingsCache.IgnoreCertificateErrors = initials["IgnoreCertificateErrors"].BoolValue;
                    UIShellSettingsCache.ShanDianHost = initials["ShanDianHost"].StringValue;
                    UIShellSettingsCache.ShanDianPort = initials["ShanDianPort"].IntValue;
                }
                else
                {
                    string UISSDir = Path.GetDirectoryName(UISSFile);

                    if (!Directory.Exists(UISSDir))
                        Directory.CreateDirectory(UISSDir);

                    UIShellSettingsCache = new SDShellSettings();

                    UIShellSettingsCache.MainListener = 8899;
                    UIShellSettingsCache.CultureName = "zh-CN";
                    UIShellSettingsCache.ShanDianHost = "127.0.0.1";
                    UIShellSettingsCache.ShanDianPort = 8899;

                    initials["MainListener"].IntValue = UIShellSettingsCache.MainListener;
                    initials["InitializeUrl"].StringValue = UIShellSettingsCache.InitializeUrl;
                    initials["AdvicesUrl"].StringValue = UIShellSettingsCache.AdvicesUrl;
                    initials["CultureName"].StringValue = UIShellSettingsCache.CultureName;
                    initials["ShowDevTools"].BoolValue = UIShellSettingsCache.ShowDevTools;
                    initials["PrintProcess"].BoolValue = UIShellSettingsCache.PrintProcess;
                    initials["IgnoreCertificateErrors"].BoolValue = UIShellSettingsCache.IgnoreCertificateErrors;
                    initials["ShanDianHost"].StringValue = UIShellSettingsCache.ShanDianHost;
                    initials["ShanDianPort"].IntValue = UIShellSettingsCache.ShanDianPort;

                    initials.SaveToFile(UISSFile);
                }
            }

            return UIShellSettingsCache;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }


    }
}
