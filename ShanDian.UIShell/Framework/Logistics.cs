using ShanDian.ClientService.Helpers;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Brower;
using ShanDian.UIShell.Framework.Helpers;
using ShanDian.UIShell.Framework.Network;
using ShanDian.UIShell.Framework.Network.Api;
using ShanDian.UIShell.Framework.Network.Service;
using ShanDian.UIShell.Framework.Server;
using ShanDian.UIShell.Properties;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ShanDian.UIShell.Framework
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public class Logistics
    {
        internal const string ApplicationInitializeName = "Initialization.exe";
        internal const string ApplicationName = "ShanDian.exe";
        internal const string LocalCultureName = "CultureName";
        internal const string ShanDianServicesName = "ShanDianServices";

        /// <summary>
        /// 安装终端Window服务
        /// </summary>
        /// <param name="printingServiceProcessName"></param>
        /// <param name="printingServiceName"></param>

        public static void IntallWindowService(string processName, string serviceName)
        {
            WindowServiceHelper.StopService(serviceName);

            WindowServiceHelper.UninstallService(processName, serviceName);

            WindowServiceHelper.InstallService(processName, serviceName);

            WindowServiceHelper.StartService(serviceName);
        }

        /// <summary>
        /// 检测Window Service是否开起
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool OpenShanDianServices()
        {
            int installedTimer = 0;
            string shanDianServicesExeFile = Path.Combine(SDShell.SDRootPath, "ShanDianServer.exe");

            while (!WindowServiceHelper.IsInstalled(ShanDianServicesName) && installedTimer < 3)
            {
                try
                {
                    WindowServiceHelper.StopService(ShanDianServicesName);

                    WindowServiceHelper.UninstallService(shanDianServicesExeFile, ShanDianServicesName);

                    WindowServiceHelper.InstallService(shanDianServicesExeFile, ShanDianServicesName);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }

                Thread.Sleep(500);

                installedTimer++;
            }

            if (!WindowServiceHelper.IsInstalled(ShanDianServicesName))
                return false;

            int startTimer = 0;

            while (!WindowServiceHelper.IsRunning(ShanDianServicesName) && startTimer < 3)
            {
                try
                {
                    WindowServiceHelper.StartService(ShanDianServicesName);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }

                Thread.Sleep(500);
                startTimer++;
            }

            return WindowServiceHelper.IsRunning(ShanDianServicesName);
        }


        /// <summary>
        /// 检测Window Service是否开起
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool ReOpenShanDianServices()
        {
            int installedTimer = 0;
            string shanDianServicesExeFile = Path.Combine(SDShell.SDRootPath, "ShanDianServer.exe");

            while (!WindowServiceHelper.IsInstalled(ShanDianServicesName) && installedTimer < 3)
            {
                try
                {
                    WindowServiceHelper.StopService(ShanDianServicesName);

                    WindowServiceHelper.UninstallService(shanDianServicesExeFile, ShanDianServicesName);

                    WindowServiceHelper.InstallService(shanDianServicesExeFile, ShanDianServicesName);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }

                Thread.Sleep(500);

                installedTimer++;
            }

            if (!WindowServiceHelper.IsInstalled(ShanDianServicesName))
                return false;

            int startTimer = 0;

            do
            {
                try
                {
                    WindowServiceHelper.StopService(ShanDianServicesName);
                    WindowServiceHelper.StartService(ShanDianServicesName);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                }

                Thread.Sleep(500);

                startTimer++;
            }
            while (!WindowServiceHelper.IsRunning(ShanDianServicesName) && startTimer < 3);

            return WindowServiceHelper.IsRunning(ShanDianServicesName);
        }

        /// <summary>
        /// 打开更新的应程
        /// </summary>
        public static void OpenRunUpgradeProcess()
        {
            string[] searchFiles = Directory.GetFiles(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "RunUpgrade.exe", SearchOption.TopDirectoryOnly);

            if (searchFiles != null && searchFiles.Length > 0)
            {
                Process process = new Process();

                ProcessStartInfo psi = new ProcessStartInfo(searchFiles[0]);

                process.StartInfo = psi;

                process.Start();

            }
            else
            {
                DebugHelper.WriteLine("RunUpgrade.exe 找不到");
            }
        }

     


        ///// <summary>
        ///// 用户登录
        ///// </summary>
        ///// <param name="username"></param>
        ///// <param name="password"></param>
        ///// <param name="rememberPassword"></param>
        ///// <param name="javascriptCallback"></param>
        //public static int Login(string username, string password, bool rememberPassword, out string errorMessage)
        //{

        //    if (UIS.Demand.RunMode == "sub")
        //    {
        //        ServiceResult<InitConfigInfo> srInitConfigInfo = ServiceAccessor.GetInitConfig();

        //        if (srInitConfigInfo == null)
        //        {
        //            errorMessage = SDResources.HttpAccessor_BadData;
        //            return 500;
        //        }
        //        else if (!srInitConfigInfo.Flag)
        //        {
        //            DebugHelper.WriteLine("获得不到主机的店信息");
        //            errorMessage = SDResources.HttpAccessor_BadData;
        //            return 500;
        //        }
        //        else if (srInitConfigInfo.Result == null)
        //        {
        //            DebugHelper.WriteLine("获得不到主机的店信息");

        //            errorMessage = SDResources.HttpAccessor_BadData;
        //            return 500;
        //        }

        //        InitConfigInfo initConfigInfo = srInitConfigInfo.Result;

        //        if (string.IsNullOrWhiteSpace(initConfigInfo.AppId) || string.IsNullOrWhiteSpace(initConfigInfo.OrgExtCode))
        //        {
        //            DebugHelper.WriteLine("获得不到主机的店信息");

        //            errorMessage = SDResources.HttpAccessor_BadData;
        //            return 500;
        //        }

        //        UIS.Demand.AppId = initConfigInfo.AppId;
        //        UIS.Demand.SecureKey = initConfigInfo.SecureKey;
        //        UIS.IsNewRestId = initConfigInfo.IsNewRestId;
        //        UIS.Demand.MikeRestId = initConfigInfo.OrgExtCode;
        //    }


        //    ServiceResult<UserInfo> hrUserInfo = ServiceAccessor.Authorized(username, password);

        //    if (hrUserInfo == null)
        //    {
        //        errorMessage = SDResources.HttpAccessor_BadData;

        //        return 500;
        //    }
        //    else if (!hrUserInfo.Flag)
        //    {
        //        errorMessage = SDResources.HttpAccessor_SystemBusy;

        //        if (ErrorHandler.Exist(hrUserInfo.Code))
        //            errorMessage = ErrorHandler.Get(hrUserInfo.Code);

        //        return hrUserInfo.Code;
        //    }
        //    else if (hrUserInfo.Result == null)
        //    {
        //        errorMessage = SDResources.HttpAccessor_NotFoundTokenInfo;

        //        return hrUserInfo.Code;
        //    }

        //    UserInfo userInfo = hrUserInfo.Result;

        //    List<CoreUserPermission> coreUserPermissions = new List<CoreUserPermission>();

        //    if (userInfo.Permissions != null && userInfo.Permissions.Count > 0)
        //    {
        //        foreach (PermissionInfo permissionInfo in userInfo.Permissions)
        //        {
        //            CoreUserPermission coreUserPermission = new CoreUserPermission();

        //            coreUserPermission.Code = permissionInfo.Code;
        //            coreUserPermission.Icon = permissionInfo.Icon;
        //            coreUserPermission.Name = permissionInfo.Name;
        //            coreUserPermission.CategoryCode = permissionInfo.CategoryCode;
        //            coreUserPermission.IsInner = permissionInfo.IsInner;
        //            coreUserPermission.Sort = permissionInfo.Sort;
        //            coreUserPermission.Token = permissionInfo.Token;
        //            coreUserPermission.Url = permissionInfo.Url;
        //            coreUserPermission.Icon = permissionInfo.Icon;
        //            coreUserPermission.KindCode = permissionInfo.KindCode;

        //            coreUserPermissions.Add(coreUserPermission);
        //        }
        //    }


        //    CoreUserInfo coreUserInfo = new CoreUserInfo();

        //    coreUserInfo.UserId = userInfo.UserId;
        //    coreUserInfo.UserName = userInfo.UserName;
        //    coreUserInfo.Token = userInfo.Token;
        //    coreUserInfo.Pwd = userInfo.Pwd;
        //    coreUserInfo.Status = userInfo.Status;
        //    coreUserInfo.UserName = userInfo.UserName;
        //    coreUserInfo.RoleName = userInfo.RoleName;
        //    coreUserInfo.Sex = userInfo.Sex;
        //    coreUserInfo.IsAdmin = userInfo.IsAdmin;
        //    coreUserInfo.Mobile = userInfo.Mobile;
        //    coreUserInfo.IsFirst = userInfo.IsFirstLogin;

        //    coreUserInfo.Permissions.AddRange(coreUserPermissions);

        //    string coreUserInfoValue = JsonConvert.SerializeObject(coreUserInfo);

        //    CDBHelper.Add("user_auth", coreUserInfoValue);

        //    string licenseNumberValues = CDBHelper.Get("license_number");

        //    if (!string.IsNullOrWhiteSpace(licenseNumberValues))
        //    {
        //        List<LicenseNumber> licensenumbers = JsonConvert.DeserializeObject<List<LicenseNumber>>(licenseNumberValues);

        //        if (licensenumbers != null)
        //        {
        //            LicenseNumber licenseNumber = licensenumbers.FirstOrDefault(l => l.Username == username);

        //            if (licenseNumber == null)
        //            {
        //                licensenumbers.Insert(0, new LicenseNumber()
        //                {
        //                    Username = username,
        //                    Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty,
        //                    IsMemberkPassword = rememberPassword
        //                });

        //                if (licensenumbers.Count > 5)
        //                {
        //                    licensenumbers.RemoveRange(5, licensenumbers.Count - 5);
        //                }

        //                CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));
        //            }
        //            else
        //            {
        //                if (licensenumbers[0].Username != username)
        //                {
        //                    licensenumbers.Remove(licenseNumber);
        //                    licensenumbers.Insert(0, licenseNumber);
        //                }

        //                licenseNumber.Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty;
        //                licenseNumber.IsMemberkPassword = rememberPassword;

        //                CDBHelper.Modify("license_number", JsonConvert.SerializeObject(licensenumbers));
        //            }
        //        }
        //    }
        //    else
        //    {
        //        List<LicenseNumber> licensenumbers = new List<LicenseNumber>()
        //            {
        //                new LicenseNumber()
        //                {
        //                    Username = username,
        //                    Password = rememberPassword ? PasswordHelper.Decrypt(password) : string.Empty,
        //                    IsMemberkPassword = rememberPassword
        //                }
        //            };

        //        CDBHelper.Add("license_number", JsonConvert.SerializeObject(licensenumbers));
        //    }

        //    errorMessage = "";

        //    return 200;
        //}


        public static bool Logout()
        {
            string coreUserInfoValue = CDBHelper.Get("user_auth");

            //if (!string.IsNullOrWhiteSpace(coreUserInfoValue))
            //{
            //    try
            //    {
            //        CoreUserInfo coreUserInfo = JsonConvert.DeserializeObject<CoreUserInfo>(coreUserInfoValue);

            //        if (coreUserInfo != null)
            //        {

            //            string url = string.Format(ServiceAccessor.HttpAccessTemplate, ChromiumSettings.Demand.Server, ChromiumSettings.Port, ServiceModule.Account, ServiceAccessor.Version, "users/token");

            //            DebugHelper.WriteLine("开始访问URL({0})", url);

            //            url = url + "?token=" + coreUserInfo.Token;

            //            var loader = new CloudLoader();
            //            var header = loader.CreateHeadDictionary("1.0.0.0", null, null);
            //            var client = new HttpSender(url, "", HttpMethod.Get, header);
            //            string result = client.GetResponse();

            //            DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, !string.IsNullOrWhiteSpace(result) ? result : "null");

            //            return true;

            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        DebugHelper.WriteException(ex);
            //    }
            //}

            return false;
        }

        /// <summary>
        /// 去掉不必要的，用于比较
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }


        /// <summary>
        /// 网页加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSign(string userId, string userName, string phone, string currentTime)
        {
            return Logistics.MD5Hash(userId + userName + phone + currentTime + SDShell.Demand.SecureKey);
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

        /// <summary>
        /// 用于激活的标名
        /// </summary>
        /// <param name="pcid"></param>
        /// <param name="shopCode"></param>
        /// <param name="currentVersion"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string GetActivitySign(string pcid, string shopCode, string currentVersion, long ts)
        {

            var str = $"pcid={pcid}&shopcode={shopCode}&currentversion={currentVersion}&ts={ts}&hahaha";
            return MD5Hash(str.ToLower());
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


        public static bool UpLoadMachine(string code, string localIp, int typeValue)
        {
            try
            {
                ServiceResult<string> serviceResult = ServiceAccessor.InsertMachine(code, "", typeValue, localIp);

                if (serviceResult == null || !serviceResult.Flag)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }

            return false;
        }

        public static HRestInfo ActivitedStore(string pcid, string shopCode)
        {
            long ts = (long)((DateTime.Now - TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local)).TotalMilliseconds);

            string currentVersion = ServerHelper.GetServerVersion() ?? "";


            string data = JsonConvert.SerializeObject(new
            {
                PCID = pcid,
                ShopCode = shopCode,
                CurrentVersion = currentVersion,
                Ts = ts,
                Sign = GetActivitySign(pcid, shopCode, currentVersion, ts)
            });

            //CloudLoader cloudLoader = new CloudLoader();

            //ProxyInfo proxyInfo = ChromiumSettings.GetProxyInfo();

            //if (proxyInfo != null)
            //    cloudLoader.SetWebProxy(proxyInfo.Server, Convert.ToInt32(proxyInfo.Port));

            //return cloudLoader.CallHttp<HRestInfo>(string.Format("{0}/v1/install/activity", HydraSystemConfig.SdApiHost), HttpMethod.Post, data);

            return null;
        }

        /// <summary>
        /// 根据IP址地判断机子是否装安过主机
        /// </summary>
        /// <param name="ipString"></param>
        /// <returns></returns>
        public static bool IsInstallByIp(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
                return false;

            //try
            //{
            //    string url = string.Format(ServiceAccessor.HttpAccessTemplate, ipString, ChromiumSettings.Port, ServiceModule.Parts, ServiceAccessor.Version, "GetInstalled");

            //    DebugHelper.WriteLine("开始访问URL({0})", url);
            //    var loader = new CloudLoader();
            //    var header = loader.CreateHeadDictionary("1.0.0.0", null, "");
            //    var client = new HttpSender(url, "", HttpMethod.Get, header);
            //    string result = client.GetResponse();

            //    DebugHelper.WriteLine("结束访问URL({0})  返回结果：{1}", url, !string.IsNullOrWhiteSpace(result) ? result : "null");
            //    return true;
            //}
            //catch (Exception ex)
            //{

            //}
            return false;
        }

        /// <summary>
        /// 根据搜索的条件找开
        /// </summary>
        /// <param name="searchDir"></param>
        /// <returns></returns>
        private static string[] GetDirectoryNames(string searchDir)
        {
            return Directory.GetDirectories(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, searchDir, SearchOption.TopDirectoryOnly);
        }


        public static void UploadSoftware()
        {
            try
            {
                SoftwareInfo softwareInfo = new SoftwareInfo();

                softwareInfo.CurrentVersion = ServerHelper.GetServerVersion();
                softwareInfo.PCID = SDShell.Demand.PCID;
                softwareInfo.SystemBit = Environment.Is64BitOperatingSystem ? "64G" : "32G";
                softwareInfo.RestSystem = ServerHelper.GetRestSystem();
                softwareInfo.IsAutoCheckout = ServerHelper.GetAutoCheckout();
                softwareInfo.OperationSystem = OSHelper.Name + " " + OSHelper.Edition;
                softwareInfo.MainIp = GetLocalIPAddress();
                softwareInfo.CPU = HardwareHelper.GetCPUName();
                softwareInfo.Memory = HardwareHelper.GetPhysicalMemory();

                ServiceResult<List<MachineInfo>> srMachines = ServiceAccessor.GetAllMachines();

                if (srMachines == null || !srMachines.Flag)
                    softwareInfo.ExtensionCount = -1;
                else
                    softwareInfo.ExtensionCount = srMachines.Result == null ? 0 : srMachines.Result.Count(t => t.Type == 20);

                softwareInfo.InstallFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
                softwareInfo.DiskSpace = new DriveInfo(Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory)).AvailableFreeSpace / 1024 + "MB";

                int timer = 0;

                while (timer++ < 3 && ApiAccessor.UploadSoftware(softwareInfo).Code != 0) ;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteException(ex);
            }
        }

        public static void LoginNoticeUpdate()
        {
            ServiceAccessor.LoginNoticeUpdate();
        }

        private static string GetLocalIPAddress()
        {
            List<string> localIpList = new List<string>();

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIpList.Add(ip.ToString());
                }
            }

            return string.Join(",", localIpList);
        }



        //public Dictionary<string, string> GetInitConfig()
        //{
        //    ServiceResult<InitConfigInfo> serviceResult = ServiceAccessor.GetInitConfig();

        //    if (serviceResult != null)
        //    {

        //    }
        //}
    }
}
