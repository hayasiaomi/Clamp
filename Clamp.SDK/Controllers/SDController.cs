using Newtonsoft.Json;
using Clamp.AddIns;
using Clamp.Common;
using Clamp.Common.HTTP;
using Clamp.Common.Model;
using Clamp.Common.Helpers;
using Clamp.SDK.Listeners;
using Clamp.SDK.Model;
using Clamp.SDK.Framework;
using Clamp.SDK.Framework.SDAPI;
using Clamp.SDK.Framework.Services;
using Clamp.SDK.Framework.Helpers;
using Clamp.SDK.Services;
using Clamp.SDK.ViewModel;
using Clamp.Webwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clamp.Common.Extensions;
using Clamp.SDK.Framework.Advices;
using Clamp.Common.Commands;
using Clamp.SDK.Framework.Model;
using Clamp.Webwork.Responses;
using Clamp.SDK.Updater;

namespace Clamp.SDK
{
    public class SDController : Controller
    {
        public SDController() : base("sd")
        {
            //用于获得善点的系统信息
            Get["Status"] = x =>
            {
                VMSDInfo systemInfo = new VMSDInfo();

                systemInfo.Version = RevisionClass.FullVersion;
                systemInfo.IsActivited = SDHelper.IsActivited();

                return JsonConvert.SerializeObject(systemInfo);
            };

            Post["Demand"] = x =>
            {
                SDDemand demand = SDHelper.GetDemand();

                if (demand != null)
                {
                    VMDemand vMDemand = new VMDemand();

                    vMDemand.PCID = demand.PCID;
                    vMDemand.AppId = demand.AppId;
                    vMDemand.MikeRestId = demand.MikeRestId;
                    vMDemand.SecureKey = demand.SecureKey;
                    vMDemand.RunMode = demand.RunMode;

                    return JsonConvert.SerializeObject(vMDemand);
                }

                return null;
            };

            //用于获得是否安装过善点
            Get["IsInstalled"] = x =>
            {
                SD.Log.Info("开始获得软件是否安装过");

                VMIsInstalled vmIsInstalled = new VMIsInstalled();

                if (SDHelper.IsActivited())
                {
                    vmIsInstalled.IsInstalled = true;
                    vmIsInstalled.Version = RevisionClass.FullVersion;
                    vmIsInstalled.RunMode = SDHelper.GetDemand().RunMode;
                }
                else
                {
                    vmIsInstalled.IsInstalled = false;
                }
                SD.Log.Info("结束获得软件是否安装过");

                return JsonConvert.SerializeObject(vmIsInstalled);
            };

            //用于活激关点系统
            Get["Activited"] = x =>
            {
                SDResponse<VMActivitedResult> response = new SDResponse<VMActivitedResult>();

                SD.Log.Info("开始激活软件");

                string storeId = this.Request.Query["storeId"];

                if (!string.IsNullOrWhiteSpace(storeId))
                {
                    IInstallService installService = SD.GetRequiredInstance<IInstallService>();

                    SDApiResponse<SDApiDemand> apiResponse = installService.ActivitedStore(storeId);

                    if (apiResponse.IsSuccess())
                    {
                        if (apiResponse.Data != null)
                        {
                            SDApiDemand apiDemand = apiResponse.Data;

                            VMActivitedResult activitedResult = new VMActivitedResult();

                            activitedResult.ActiveTime = apiDemand.ActiveTime;
                            activitedResult.IsBind = apiDemand.IsBunding;
                            activitedResult.MainIp = apiDemand.MainIp;
                            activitedResult.Online = apiDemand.Online;

                            response.Result = activitedResult;
                        }
                    }
                }

                SD.IsActivited = false;

                SD.Log.Info("结束激活软件");

                return response.SerializeObject();
            };


            //用于只有主机才可以有安装分机
            Post["InstallSub"] = x =>
            {
                VMInstallSub vMInstallSub = new VMInstallSub();

                string pcid = this.Request.Form["pcid"];
                string subIp = this.Request.Form["subIp"];
                string subListener = this.Request.Form["subListener"];

                if (string.IsNullOrWhiteSpace(pcid) || string.IsNullOrWhiteSpace(subIp) || string.IsNullOrWhiteSpace(subListener))
                {
                    vMInstallSub.IsIntalled = false;
                    vMInstallSub.Mistake = "参数不正确";

                    return JsonConvert.SerializeObject(vMInstallSub);
                }

                if (!SDHelper.IsMain())
                {
                    vMInstallSub.IsIntalled = false;
                    vMInstallSub.Mistake = "分机没有安装子机的功能";

                    return JsonConvert.SerializeObject(vMInstallSub);
                }

                IInstallService installService = SD.GetRequiredInstance<IInstallService>();

                InstallSubResult installSubResult = installService.InstallSub(pcid, subIp, Convert.ToInt32(subListener));

                vMInstallSub.IsIntalled = installSubResult.IsIntalled;
                vMInstallSub.Mistake = installSubResult.Mistake;

                return JsonConvert.SerializeObject(vMInstallSub);
            };


            //用于UI端发来的安装分机
            Get["SetupSub"] = x =>
            {
                VMSetupSub setupSub = new VMSetupSub();

                string mainIp = this.Request.Query["mainIp"];

                if (!string.IsNullOrWhiteSpace(mainIp))
                {
                    IInstallService installService = SD.GetRequiredInstance<IInstallService>();

                    SetupSubResult installSubResult = installService.SetupSub(mainIp);

                    setupSub.IsIntalled = installSubResult.IsIntalled;
                    setupSub.ErrorMessage = installSubResult.Mistake;
                }
                else
                {
                    setupSub.IsIntalled = false;
                    setupSub.ErrorMessage = "主机的IP不正确";
                }

                return JsonConvert.SerializeObject(setupSub);
            };


            Post["command"] = x =>
            {
                string commandName = this.Request.Form.CmdName;
                string data = this.Request.Form.Data;

                if (string.IsNullOrWhiteSpace(commandName))
                {
                    return JsonConvert.SerializeObject(new CommandResult() { IsSucceed = false, Mistake = "命令参数不能为空" });
                }

                return JsonConvert.SerializeObject(SDCmdManager.Handle(commandName, data));
            };

            Post["marks/add"] = x =>
            {
                string value = this.Request.Form.Value;
                string name = this.Request.Form.Name;
                string groupName = this.Request.Form.GroupName;

                MarkService shanDianService = ObjectSingleton.GetRequiredInstance<MarkService>();

                shanDianService.AddMark(name, value, groupName);

                return null;
            };

            Post["marks/get"] = x =>
            {
                string name = this.Request.Form.Name;
                string groupName = this.Request.Form.GroupName;

                MarkService shanDianService = ObjectSingleton.GetRequiredInstance<MarkService>();

                string dataValue = Convert.ToString(shanDianService.GetValueByName(name, groupName));

                return dataValue;
            };

            Get["printers"] = x =>
            {
                List<VMPrinterinfo> vMPrinterinfos = new List<VMPrinterinfo>();

                IWinFormService winFormService = SD.GetRequiredInstance<IWinFormService>();

                List<LocalPrinterInfo> localPrinterInfos = winFormService.GetLocalPrinterInfos();

                foreach (LocalPrinterInfo localPrinterInfo in localPrinterInfos)
                {
                    VMPrinterinfo vMPrinterinfo = new VMPrinterinfo();

                    vMPrinterinfo.PrintName = localPrinterInfo.PrintName;
                    vMPrinterinfo.State = localPrinterInfo.State;

                    vMPrinterinfos.Add(vMPrinterinfo);
                }

                return JsonConvert.SerializeObject(vMPrinterinfos);
            };

            Get["upgrades/check"] = x =>
            {
                UpdateInfo updateInfo = new UpdateInfo();

                updateInfo.VersionCode = RevisionClass.FullVersion;

                updateInfo.UpdateLog = string.Empty;

                updateInfo.DownloadUrl = string.Empty;

                return JsonConvert.SerializeObject(updateInfo);
            };
        }
    }
}
