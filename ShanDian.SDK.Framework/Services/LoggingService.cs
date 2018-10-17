using ShanDian.SDK.Framework;
using ShanDian.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print
{
    public static class LoggingService
    {
        static ILoggingService Service
        {
            get { return ObjectSingleton.GetRequiredInstance<ILoggingService>(); }
        }

        public static void Debug(string message)
        {
            Service.Debug(message);
        }

        public static void DebugFormatted(string format, params object[] args)
        {
            Service.DebugFormatted(format, args);
        }

        public static void Info(string message)
        {
            Service.Info(message);
        }

        public static void InfoFormatted(string format, params object[] args)
        {
            Service.InfoFormatted(format, args);
        }

        public static void Warn(string message)
        {
            Service.Warn(message);
        }

        public static void Warn(string message, Exception exception)
        {
            Service.Warn(message, exception);
        }

        public static void WarnFormatted(string format, params object[] args)
        {
            Service.WarnFormatted(format, args);
        }

        public static void Error(string message)
        {
            Service.Error(message);
        }

        public static void Error(string message, Exception exception)
        {
            Service.Error(message, exception);
        }

        public static void ErrorFormatted(string format, params object[] args)
        {
            Service.ErrorFormatted(format, args);
        }

        public static void Fatal(string message)
        {
            Service.Fatal(message);
        }

        public static void Fatal(string message, Exception exception)
        {
            Service.Fatal(message, exception);
        }

        public static void FatalFormatted(string format, params object[] args)
        {
            Service.FatalFormatted(format, args);
        }

        public static bool IsDebugEnabled
        {
            get
            {
                return Service.IsDebugEnabled;
            }
        }

        public static bool IsInfoEnabled
        {
            get
            {
                return Service.IsInfoEnabled;
            }
        }

        public static bool IsWarnEnabled
        {
            get
            {
                return Service.IsWarnEnabled;
            }
        }

        public static bool IsErrorEnabled
        {
            get
            {
                return Service.IsErrorEnabled;
            }
        }

        public static bool IsFatalEnabled
        {
            get
            {
                return Service.IsFatalEnabled;
            }
        }
    }
}
