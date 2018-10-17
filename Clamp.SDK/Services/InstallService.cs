using Microsoft.Win32;
using Newtonsoft.Json;
using Clamp.Common.HTTP;
using Clamp.Common.Model;
using Clamp.SDK.Listeners;
using Clamp.SDK.Model;
using Clamp.SDK.Framework;
using Clamp.SDK.Framework.Model;
using Clamp.SDK.Framework.SDAPI;
using Clamp.SDK.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Clamp.SDK.Framework.Services;
using Clamp.SDK.ViewModel;
using Clamp.Common.Helpers;
using Clamp.AddIns;
using System.Threading;
using System.IO;
using Clamp.Common.Initial;

namespace Clamp.SDK.Services
{
    public class InstallService : IInstallService
    {
        private readonly ShanDianConfiguraction shanDianConfiguraction;
        private readonly MediumConfiguration mediumConfiguration;
        private readonly IMachineService machineService;
        private readonly IPrinterService printerService;

        public InstallService()
        {
            this.shanDianConfiguraction = ObjectSingleton.GetRequiredInstance<ShanDianConfiguraction>();
            this.mediumConfiguration = ObjectSingleton.GetRequiredInstance<MediumConfiguration>();
            this.machineService = ObjectSingleton.GetRequiredInstance<IMachineService>();
            this.printerService = ObjectSingleton.GetRequiredInstance<IPrinterService>();
        }
        /// <summary>
        /// 获得注册表的GUID。
        /// </summary>
        /// <returns></returns>
        public string GetRegistryPCID()
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
        public void SaveRegistryPCID(string pcid)
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
        public void SaveMerchantNo(string merchantNo)
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
        /// 激活门店
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public SDApiResponse<SDApiDemand> ActivitedStore(string storeId)
        {
            string pcid = this.GetRegistryPCID();

            if (string.IsNullOrWhiteSpace(pcid))
            {
                pcid = Guid.NewGuid().ToString("N");
            }

            long ts = (long)((DateTime.Now - TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local)).TotalMilliseconds);

            string data = JsonConvert.SerializeObject(new
            {
                PCID = pcid,
                ShopCode = storeId,
                CurrentVersion = RevisionClass.FullVersion,
                Ts = ts,
                Sign = GetActivitySign(pcid, storeId, RevisionClass.FullVersion, ts)
            });

            int timer = 0;

            do
            {
                HttpResponse httpResponse = SDApiHelper.SDRequest(SDApiHelper.GetSDApiUrl("/v1/install/activity"), HttpMethod.POST, data);

                if (httpResponse != null)
                {
                    SDApiResponse<SDApiDemand> apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<SDApiDemand>>();

                    if (apiResponse != null && (apiResponse.Code == 0 || apiResponse.Code == 200))
                    {
                        if (apiResponse.Data != null)
                        {
                            if (!apiResponse.Data.IsBunding)
                            {
                                try
                                {
                                    SDApiDemand apiDemand = apiResponse.Data;

                                    //保存激活凭证
                                    SDDemand demand = new SDDemand();

                                    demand.PCID = pcid;
                                    demand.AppId = apiDemand.AppId;
                                    demand.MikeRestId = apiDemand.MikeRestId;
                                    demand.SecureKey = apiDemand.SecureKey;
                                    demand.RunMode = "main";

                                    SDHelper.SetupDemandFile(demand);


                                    //设置客户端的服务配置
                                    string sdshellConfigPath = Path.Combine(SD.GetSDRootPath(), this.shanDianConfiguraction.SDShellPathName, Constants.SDShellConfigurationName);

                                    InitialFile initialFile = InitialFile.LoadFromFile(sdshellConfigPath);

                                    initialFile["ShanDianServer"].StringValue = "127.0.0.1";
                                    initialFile["ShanDianPort"].IntValue = this.mediumConfiguration.MainListener;

                                    initialFile.SaveToFile(sdshellConfigPath);

                                    //保存餐厅的信息
                                    Store store = new Store();

                                    store.Address = apiDemand.Address;
                                    store.AppId = apiDemand.AppId;
                                    store.BrandId = apiDemand.BrandId;
                                    store.BrandName = apiDemand.BrandName;
                                    store.Logo = apiDemand.Logo;
                                    store.MikeRestId = apiDemand.MikeRestId;
                                    store.Name = apiDemand.Name;
                                    store.Phone = apiDemand.Phone;
                                    store.SubName = apiDemand.SubName;
                                    store.ActiveTime = apiDemand.ActiveTime;

                                    this.machineService.SetupStore(store);

                                    //上传本机的信息
                                    this.machineService.SetupComputer(pcid, "主机", "127.0.0.1", this.mediumConfiguration.MainListener, demand.RunMode);

                                    this.SaveRegistryPCID(pcid);

                                    SDDemand localDemand = SDHelper.GetDemand();

                                    if (localDemand != null)
                                    {


                                        SD.Log.Info("开始安装管道通信");

                                        SD.SDExecutor.InstallPipelines();

                                        SD.Log.Info("结束安装管道通信");

                                        SD.Log.Info("开始更新打印机列表");

                                        TaskHelper.Run(() =>
                                        {
                                            IWinFormService winFormService = ObjectSingleton.GetRequiredInstance<IWinFormService>();

                                            List<LocalPrinterInfo> localPrinterInfos = winFormService.GetLocalPrinterInfos();

                                            foreach (LocalPrinterInfo localPrinterInfo in localPrinterInfos)
                                            {
                                                PrinterInfo printerInfo = new PrinterInfo();

                                                printerInfo.PCID = pcid;
                                                printerInfo.Port = this.mediumConfiguration.MainListener;
                                                printerInfo.IP = "127.0.0.1";
                                                printerInfo.PrintName = localPrinterInfo.PrintName;
                                                printerInfo.State = localPrinterInfo.State;
                                                printerInfo.Enable = 1;
                                                printerInfo.ModifyTime = DateTime.Now;

                                                printerService.AddPrinter(printerInfo);
                                            }

                                        });

                                        SD.Log.Info("结束更新打印机列表");


                                        //主机初始化
                                        if (string.Equals(localDemand.RunMode, "main", StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            if (!SD.IsActivited)
                                            {
                                                try
                                                {
                                                    SD.Log.Info("开始初始化模块管理器");

                                                    AddInManager.Initialize();

                                                    SD.Log.Info("结束初始化模块管理器");

                                                    SD.Log.Info("开始安装模块内的HTTP功能");

                                                    SD.SDExecutor.InstallAddIns();

                                                    SD.Log.Info("结束安装模块内的HTTP功能");

                                                    SD.IsActivited = true;
                                                }
                                                catch (Exception ex)
                                                {
                                                    SD.Log.Error("主机初始化时候出错", ex);
                                                    SD.IsActivited = false;
                                                    SDHelper.UnInstallDemandFile();
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SD.IsActivited = false;
                                        SDHelper.UnInstallDemandFile();
                                    }
                                }
                                catch (Exception)
                                {
                                    SD.IsActivited = false;
                                    SDHelper.UnInstallDemandFile();

                                    apiResponse.Code = 408;
                                    apiResponse.Msg = "远程数据访问成功，但是本地文件激活失败";
                                    apiResponse.Ts = DateTime.Now.Ticks;

                                    return apiResponse;
                                }
                            }

                            return apiResponse;
                        }
                    }
                }

                timer++;

                Thread.Sleep(800);
            }
            while (timer < 3);


            return null;
        }

        /// <summary>
        /// 安装分机
        /// </summary>
        /// <param name="mainIp"></param>
        /// <returns></returns>
        public SetupSubResult SetupSub(string mainIp)
        {
            SetupSubResult setupSubResult = new SetupSubResult();

            string pcid = this.GetRegistryPCID();

            try
            {
                if (!string.IsNullOrWhiteSpace(mainIp))
                {
                    string[] hosts = mainIp.Split(',');

                    if (hosts != null && hosts.Length > 0)
                    {
                        foreach (string host in hosts)
                        {
                            InstalledResult installedResult = this.IsInstallByIp(host, this.mediumConfiguration.MainListener);

                            if (installedResult != null && installedResult.IsInstalled && string.Equals("main", installedResult.RunMode, StringComparison.CurrentCultureIgnoreCase))
                            {

                                SDDemand demand = new SDDemand();

                                demand.PCID = pcid;
                                demand.Server = host;
                                demand.RunMode = "sub";

                                List<string> addrs = Helper.GetComputerLanIP();

                                if (addrs.Count < 0)
                                {
                                    setupSubResult.IsIntalled = false;
                                    setupSubResult.Mistake = "没有找到激活的主机";

                                    return setupSubResult;
                                }

                                BegInstallSubResult installSubResult = this.BegInstallSub(host, pcid, string.Join(",", addrs), this.mediumConfiguration.MainListener);

                                if (installSubResult.IsIntalled)
                                {
                                    SDHelper.SetupDemandFile(demand);

                                    this.SaveRegistryPCID(pcid);

                                    SD.Log.Info("开始安装管道通信");

                                    SD.SDExecutor.InstallPipelines();

                                    SD.Log.Info("结束安装管道通信");

                                    setupSubResult.IsIntalled = true;

                                    //设置客户端的服务配置
                                    string sdshellConfigPath = Path.Combine(SD.GetSDRootPath(), this.shanDianConfiguraction.SDShellPathName, Constants.SDShellConfigurationName);

                                    InitialFile initialFile = InitialFile.LoadFromFile(sdshellConfigPath);

                                    initialFile["ShanDianHost"].StringValue = host;
                                    initialFile["ShanDianPort"].IntValue = this.mediumConfiguration.MainListener;

                                    initialFile.SaveToFile(sdshellConfigPath);
                                }
                                else
                                {
                                    setupSubResult.IsIntalled = false;
                                    setupSubResult.Mistake = installSubResult.Mistake;
                                }

                                return setupSubResult;
                            }
                        }
                    }

                    setupSubResult.IsIntalled = false;
                    setupSubResult.Mistake = "没有找到激活的主机";
                }
            }
            catch (Exception ex)
            {
                setupSubResult.IsIntalled = false;
                setupSubResult.Mistake = ex.Message;
            }

            return setupSubResult;
        }

        /// <summary>
        /// 用于激活的标名
        /// </summary>
        /// <param name="pcid"></param>
        /// <param name="shopCode"></param>
        /// <param name="currentVersion"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        private string GetActivitySign(string pcid, string shopCode, string currentVersion, long ts)
        {
            var str = $"pcid={pcid}&shopcode={shopCode}&currentversion={currentVersion}&ts={ts}&hahaha";
            return MD5Hash(str.ToLower());
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string MD5Hash(string input)
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
        /// 判断当前的IP是否安装过主机
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        private InstalledResult IsInstallByIp(string host, int listener)
        {
            SD.Log.Info("通过IP判断是否安装善点");

            int timer = 0;

            do
            {
                SD.Log.Info($"安装过机子-http://{host}:{listener}/sd/IsInstalled");

                HttpRequest httpRequest = new HttpRequest($"http://{host}:{listener}/sd/IsInstalled", HttpMethod.GET);

                httpRequest.Timeout = 3000;

                HttpResponse httpResponse = httpRequest.GetHttpResponse();

                if (httpResponse != null)
                {
                    VMIsInstalled vMIsInstalled = httpResponse.AsDeserializeBody<VMIsInstalled>();

                    if (vMIsInstalled != null)
                    {
                        InstalledResult installedResult = new InstalledResult();

                        installedResult.IsInstalled = vMIsInstalled.IsInstalled;
                        installedResult.RunMode = vMIsInstalled.RunMode;
                        installedResult.Version = vMIsInstalled.Version;

                        return installedResult;
                    }

                    return null;
                }

                timer++;

                Thread.Sleep(800);

            } while (timer < 3);


            return null;

        }

        public InstallSubResult InstallSub(string pcid, string subIp, int subListener)
        {
            SD.Log.Info("安装分机");

            InstallSubResult installSubResult = new InstallSubResult();

            string[] addrs = subIp.Split(',');

            foreach (string addr in addrs)
            {
                InstalledResult installedResult = this.IsInstallByIp(addr, subListener);

                if (installedResult != null)
                {
                    this.machineService.SetupComputer(pcid, "分机", addr, subListener, "sub");

                    //去找分机的打印机
                    TaskHelper.Run(() =>
                    {
                        List<PrinterInfo> printerInfos = this.GetPrinterInfosByPCID(addr, subListener);

                        foreach (PrinterInfo printerInfo in printerInfos)
                        {
                            printerInfo.PCID = pcid;

                            this.printerService.AddPrinter(printerInfo);
                        }

                    });

                    installSubResult.IsIntalled = true;

                    return installSubResult;
                }
            }

            installSubResult.IsIntalled = false;
            installSubResult.Mistake = $"分机{pcid}的通信有问题";

            return installSubResult;
        }

        public List<PrinterInfo> GetPrinterInfosByPCID(string addr, int subListener)
        {
            List<PrinterInfo> printerInfos = new List<PrinterInfo>();

            SD.Log.Info("请求安装分机");

            int timer = 0;
            do
            {
                HttpRequest httpRequest = new HttpRequest($"http://{addr}:{subListener}/sd/printers", HttpMethod.GET);

                HttpResponse httpResponse = httpRequest.GetHttpResponse();

                if (httpResponse != null)
                {
                    List<VMPrinterinfo> vMPrinterinfos = httpResponse.AsDeserializeBody<List<VMPrinterinfo>>();

                    if (vMPrinterinfos != null)
                    {
                        foreach (VMPrinterinfo vMPrinterinfo in vMPrinterinfos)
                        {
                            PrinterInfo printerInfo = new PrinterInfo();

                            printerInfo.IP = addr;
                            printerInfo.Port = subListener;
                            printerInfo.PrintName = vMPrinterinfo.PrintName;
                            printerInfo.State = vMPrinterinfo.State;
                            printerInfo.Enable = 1;
                            printerInfo.ModifyTime = DateTime.Now;
                        }
                    }

                    break;
                }

                timer++;

                Thread.Sleep(800);

            } while (timer < 3);

            return printerInfos;
        }

        public BegInstallSubResult BegInstallSub(string host, string pcid, string subIp, int subListener)
        {
            SD.Log.Info("请求安装分机");

            BegInstallSubResult begInstallSubResult = new BegInstallSubResult();

            int timer = 0;
            do
            {
                HttpRequest httpRequest = new HttpRequest($"http://{host}:{mediumConfiguration.MainListener}/sd/InstallSub", HttpMethod.POST);

                httpRequest.Data = $"pcid={pcid}&subIp={subIp}&subListener={subListener}";

                HttpResponse httpResponse = httpRequest.GetHttpResponse();

                if (httpResponse != null)
                {
                    VMInstallSub vMInstallSub = httpResponse.AsDeserializeBody<VMInstallSub>();

                    if (vMInstallSub != null)
                    {
                        begInstallSubResult.IsIntalled = vMInstallSub.IsIntalled;
                        begInstallSubResult.Mistake = vMInstallSub.Mistake;
                    }
                    else
                    {
                        begInstallSubResult.IsIntalled = false;
                        begInstallSubResult.Mistake = "返回的数据格不正确";
                    }

                    return begInstallSubResult;
                }

                timer++;

                Thread.Sleep(800);

            } while (timer < 3);

            begInstallSubResult.IsIntalled = false;
            begInstallSubResult.Mistake = "远程通信不正常";

            return begInstallSubResult;
        }
    }
}
