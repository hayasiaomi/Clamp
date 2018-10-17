using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ShanDain.AIM.Helper
{
    public class ConfigHelper
    {
        public static string HostAddress { get; private set; }
        public static string LocalBasePath { get; set; } //"./Packages";
        public static string DownloadTempPath { get; set; } //"./DownloadTemp";
        public static string DownloadPath { get; set; } //"./Download";
        public static string ServiceName { get; set; } //"Service1";
        public static string ProcessName { get; set; } //"Service.exe";

        static ConfigHelper()
        {
            HostAddress = ConfigurationManager.AppSettings[nameof(HostAddress)].Trim();
            LocalBasePath = ConfigurationManager.AppSettings[nameof(LocalBasePath)].Trim();
            DownloadTempPath = ConfigurationManager.AppSettings[nameof(DownloadTempPath)].Trim();
            DownloadPath = ConfigurationManager.AppSettings[nameof(DownloadPath)].Trim();
            ServiceName = ConfigurationManager.AppSettings[nameof(ServiceName)].Trim();
            ProcessName = ConfigurationManager.AppSettings[nameof(ProcessName)].Trim();
        }
    }
}
