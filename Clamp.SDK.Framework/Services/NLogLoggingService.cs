using NLog;
using Clamp.Common.Helpers;
using Clamp.SDK.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Services
{
    public sealed class NLogLoggingService : ILoggingService
    {
        private Logger log;

        public NLogLoggingService()
        {
            this.log = LogManager.GetLogger("ShanDain");
        }

        public void Debug(string message)
        {
            log.Debug(message);
        }

        public void DebugFormatted(string format, params object[] args)
        {
            log.Debug(CultureInfo.InvariantCulture, format, args);
        }

        public void Info(string message)
        {
            log.Info(message);
        }

        public void InfoFormatted(string format, params object[] args)
        {
            log.Info(CultureInfo.InvariantCulture, format, args);
        }

        public void Warn(string message)
        {
            log.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            log.Warn(exception, message);
        }

        public void WarnFormatted(string format, params object[] args)
        {
            log.Warn(CultureInfo.InvariantCulture, format, args);
        }

        public void Error(string message)
        {
            log.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            log.Error(exception, message);
        }

        public void ErrorFormatted(string format, params object[] args)
        {
            log.Error(CultureInfo.InvariantCulture, format, args);
        }

        public void Fatal(string message)
        {
            log.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            log.Fatal(exception, message);
        }

        public void FatalFormatted(string format, params object[] args)
        {
            log.Fatal(CultureInfo.InvariantCulture, format, args);
        }



        public bool IsDebugEnabled
        {
            get
            {
                return log.IsDebugEnabled;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return log.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return log.IsWarnEnabled;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return log.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return log.IsFatalEnabled;
            }
        }
    }
}
