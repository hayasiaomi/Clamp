using NLog;
using ShanDian.UIShell.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Services
{
    public sealed class NLogService
    {
        private static Logger Log;

        static NLogService()
        {
            Log = LogManager.GetLogger("SDShell");
        }


        public static void Debug(string message)
        {
            Log.Debug(message);
        }

        public static void DebugFormatted(string format, params object[] args)
        {
            Log.Debug(CultureInfo.InvariantCulture, format, args);
        }

        public static void Info(string message)
        {
            Log.Info(message);
        }

        public static void InfoFormatted(string format, params object[] args)
        {
            Log.Info(CultureInfo.InvariantCulture, format, args);
        }

        public static void Warn(string message)
        {
            Log.Warn(message);
        }

        public static void Warn(string message, Exception exception)
        {
            Log.Warn(exception, message);
        }

        public static void WarnFormatted(string format, params object[] args)
        {
            Log.Warn(CultureInfo.InvariantCulture, format, args);
        }

        public static void Error(string message)
        {
            Log.Error(message);
        }

        public static void Error(string message, Exception exception)
        {
            Log.Error(exception, message);
        }

        public static void ErrorFormatted(string format, params object[] args)
        {
            Log.Warn(CultureInfo.InvariantCulture, format, args);
        }

        public static void Fatal(string message)
        {
            Log.Fatal(message);
        }

        public static void Fatal(string message, Exception exception)
        {
            Log.Fatal(exception, message);
        }

        public static void FatalFormatted(string format, params object[] args)
        {
            Log.Fatal(CultureInfo.InvariantCulture, format, args);
        }



        public static bool IsDebugEnabled
        {
            get
            {
                return Log.IsDebugEnabled;
            }
        }

        public static bool IsInfoEnabled
        {
            get
            {
                return Log.IsInfoEnabled;
            }
        }

        public static bool IsWarnEnabled
        {
            get
            {
                return Log.IsWarnEnabled;
            }
        }

        public static bool IsErrorEnabled
        {
            get
            {
                return Log.IsErrorEnabled;
            }
        }

        public static bool IsFatalEnabled
        {
            get
            {
                return Log.IsFatalEnabled;
            }
        }
    }
}
