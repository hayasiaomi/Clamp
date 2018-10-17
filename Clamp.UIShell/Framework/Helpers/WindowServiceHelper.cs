using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Clamp.UIShell.Framework.Helpers
{
    public class WindowServiceHelper
    {
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

        public static bool IsRunning(string serviceName)
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

        public static void InstallService(string fileName, string serviceName)
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

        public static void UninstallService(string fileName, string serviceName)
        {
            if (!IsInstalled(serviceName))
                return;

            using (AssemblyInstaller installer = GetInstaller(fileName))
            {
                IDictionary state = new Hashtable();

                installer.Uninstall(state);
            }

        }

        public static void StartService(string serviceName)
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

        public static void StopService(string serviceName)
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
    }
}
