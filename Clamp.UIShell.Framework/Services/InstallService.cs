using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Clamp.Common.Extensions;
using Clamp.Common.HTTP;
using Clamp.UIShell.Framework.Helpers;
using Clamp.UIShell.Framework.Model;

namespace Clamp.UIShell.Framework.Services
{
    /// <summary>
    /// 关于安装过程的业务服务类
    /// </summary>
    public sealed class InstallService
    {
        /// <summary>
        /// 激活门店
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public static SDResponse<ActivitedInfo> ActivitedStore(string storeId)
        {
            int timer = 0;

            do
            {
                HttpResponse httpResponse = HttpRequest.Get(SDShellHelper.GetSelfHost($"/Activited?storeId={storeId}"));

                if (httpResponse != null)
                {
                    SDResponse<ActivitedInfo> response = httpResponse.AsDeserializeBody<SDResponse<ActivitedInfo>>();

                    if (response != null)
                    {
                        return response;
                    }
                }

                timer++;

            } while (timer < 3);

            return null;
        }

        public static InstallSubInfo InstallSub(string mainIp)
        {
            int timer = 0;

            do
            {
                HttpResponse httpResponse = HttpRequest.Get(SDShellHelper.GetSelfHost($"/SetupSub?mainIp={mainIp}"));

                if (httpResponse != null)
                {
                    InstallSubInfo installSubInfo = httpResponse.AsDeserializeBody<InstallSubInfo>();

                    if (installSubInfo != null)
                    {
                        return installSubInfo;
                    }
                }

                timer++;

            } while (timer < 3);

            return null;
        }


        public static bool IsInstalled(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        public static bool IsWindowServiceRunning(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (!IsInstalled(serviceName))
                    return false;

                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        public static AssemblyInstaller GetInstaller(string fileName)
        {
            AssemblyInstaller installer = new AssemblyInstaller(fileName, null);

            installer.UseNewContext = true;

            return installer;
        }

        public static void InstallWindowService(string fileName, string serviceName)
        {
            if (IsInstalled(serviceName))
                return;

            using (AssemblyInstaller installer = GetInstaller(fileName))
            {
                IDictionary state = new Hashtable();

                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                }
                catch (Exception ex)
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch
                    {

                    }
                    throw ex;
                }
            }

        }

        public static void UninstallWindowService(string fileName, string serviceName)
        {
            if (!IsInstalled(serviceName))
                return;

            using (AssemblyInstaller installer = GetInstaller(fileName))
            {
                IDictionary state = new Hashtable();

                installer.Uninstall(state);
            }

        }

        public static void StartWindowService(string serviceName)
        {
            if (!IsInstalled(serviceName))
                return;

            using (ServiceController controller = new ServiceController(serviceName))
            {

                if (controller.Status != ServiceControllerStatus.Running && controller.Status != ServiceControllerStatus.StartPending)
                {
                    controller.Start();
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(180));
                }
                else if (controller.Status == ServiceControllerStatus.StartPending)
                {
                    controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(180));
                }
            }
        }

        public static void StopWindowService(string serviceName)
        {
            if (!IsInstalled(serviceName))
                return;

            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (controller.Status != ServiceControllerStatus.Stopped)
                {
                    controller.Stop();
                    controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(180));
                }

            }
        }

        /// <summary>
        /// 检测Window Service是否开起
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool OpenShanDianServices()
        {
            int installedTimer = 0;
            string ShanDianServicesName = "ShanDian";
            string shanDianExeFile = Path.Combine(SDShellHelper.GetSDRootPath(), "ShanDian.exe");

            while (!IsInstalled(ShanDianServicesName) && installedTimer < 3)
            {
                try
                {
                    InstallWindowService(shanDianExeFile, ShanDianServicesName);
                }
                catch (Exception ex)
                {
                    NLogService.Error("安装善点服务失败", ex);
                }

                installedTimer++;

                Thread.Sleep(500);
            }

            if (!IsInstalled(ShanDianServicesName))
                return false;

            int startTimer = 0;

            while (!IsWindowServiceRunning(ShanDianServicesName) && startTimer < 3)
            {
                try
                {
                    StartWindowService(ShanDianServicesName);
                }
                catch (Exception ex)
                {
                    NLogService.Error("开起善点服务失败", ex);
                }

                Thread.Sleep(500);
                startTimer++;
            }

            return IsWindowServiceRunning(ShanDianServicesName);
        }
    }
}
