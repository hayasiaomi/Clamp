using Aomi.Main;
using Clamp.OSGI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Clamp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new ShanDianService()
            //};
            //ServiceBase.Run(ServicesToRun);

            IClampBundle clampBundle = ClampBundleFactory.GetClampBundle();

            clampBundle.Start();

            //foreach (ICommand cmd in clampBundle.GetExtensionObjects(typeof(ICommand)))
            //    cmd.Run();

            clampBundle.WaitForStop();

        }
    }
}
