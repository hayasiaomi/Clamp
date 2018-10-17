using Clamp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Services
{
  
    public interface ILoggingService : IService
    {

        void Debug(string message);
        void DebugFormatted(string format, params object[] args);
        void Info(string message);
        void InfoFormatted(string format, params object[] args);
        void Warn(string message);
        void Warn(string message, Exception exception);
        void WarnFormatted(string format, params object[] args);
        void Error(string message);
        void Error(string message, Exception exception);
        void ErrorFormatted(string format, params object[] args);
        void Fatal(string message);
        void Fatal(string message, Exception exception);
        void FatalFormatted(string format, params object[] args);
        bool IsDebugEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsWarnEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
    }
}
