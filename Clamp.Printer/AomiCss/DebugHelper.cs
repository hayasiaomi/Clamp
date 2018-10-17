using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Hydra.AomiCss
{
    /// <summary>
    /// 用于记录日志的帮助类
    /// </summary>
    public static class DebugHelper
    {
        public static Logger Logger { get; private set; }

        public static void Init(string logFilePath)
        {
            Logger = new Logger(logFilePath);
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string message = "")
        {
            if (Logger != null)
            {
                Logger.WriteLine(message);
            }
            else
            {
                Debug.WriteLine(message);
            }
        }
        /// <summary>
        /// 根据格式写日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        /// <summary>
        /// 根据异常信息写日志
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void WriteException(string exception, string message = "Exception")
        {
            if (Logger != null)
            {
                Logger.WriteException(exception, message);
            }
            else
            {
                Debug.WriteLine(exception);
            }
        }

        /// <summary>
        /// 根据导常对象写日志
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public static void WriteException(Exception exception, string message = "Exception")
        {
            WriteException(exception.ToString(), message);
        }
    }

  


}
