using Clamp.Common;
using Clamp.Common.Model;
using Clamp.Common.Helpers;
using Clamp.SDK.Listeners;
using Clamp.SDK.Framework;
using Clamp.SDK.Framework.Services;
using Clamp.SDK.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK
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
