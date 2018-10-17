using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using TimeoutException = System.TimeoutException;

namespace ShanDain.AIM.Helper
{
    public class ServiceHelper
    {
        //1 未安装服务, 
        public static ServiceOPResult StopService(string name)
        {
            try
            {
                using (ServiceController controller = new ServiceController(name))
                {
                    var servicestatus = controller.Status;
                    if (servicestatus != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(40));
                    }
                    return ServiceOPResult.Success;
                }
            }
            catch (InvalidOperationException ex)
            {
                return ServiceOPResult.UnInsdtalled;
            }
            catch (TimeoutException ex)
            {
                return ServiceOPResult.Timeout;
            }
            catch (Exception ex)
            {
                return ServiceOPResult.Unknow;
            }
        }

        public static bool StartService(string name, ref Exception ex)
        {
            try
            {
                using (ServiceController controller = new ServiceController(name))
                {
                    var servicestatus = controller.Status;
                    if (servicestatus != ServiceControllerStatus.Running)
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(40));
                    }

                    return true;
                }
            }//未找到服务
            catch (InvalidOperationException e)
            {
                return true;
            }//超时或者别的异常
            catch (Exception e)
            {
                ex = e;
            }

            return false;
        }
    }

    public enum ServiceOPResult
    {
        UnInsdtalled,
        Timeout,
        Success,
        Unknow
    }
}
