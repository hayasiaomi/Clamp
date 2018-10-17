using ShanDian.Common;
using ShanDian.Common.Model;
using ShanDian.Common.Helpers;
using ShanDian.SDK.Listeners;
using ShanDian.SDK.Framework;
using ShanDian.SDK.Framework.Services;
using ShanDian.SDK.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK
{
    public class SD
    {
        internal volatile static bool IsActivited = false;

        internal static SDExecutor SDExecutor
        {
            get { return GetRequiredInstance<SDExecutor>(); }
        }

        internal static T GetRequiredInstance<T>() where T : class
        {
            return ObjectSingleton.GetRequiredInstance<T>();
        }

        internal static SDContainer SDContainer
        {
            get { return GetRequiredInstance<SDContainer>(); }
        }

        internal static ILoggingService Log
        {
            get { return GetRequiredInstance<ILoggingService>(); }
        }

        internal static string GetSDRootPath()
        {
            return SDHelper.GetSDRootPath();
        }

        public static void Start()
        {
            if (SDExecutor == null)
            {
                SDExecutor executor = new SDExecutor();

                executor.Initialize();

                executor.Execute();
            }
        }

        public static void Stop()
        {
            if (SDExecutor != null)
            {
                SDExecutor.Abort();
            }
        }
    }
}
