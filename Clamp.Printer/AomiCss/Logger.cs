using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hydra.AomiCss
{
    /// <summary>
    /// 用于记录日志
    /// </summary>
    public class Logger
    {
        public delegate void MessageAddedEventHandler(string message);

        public event MessageAddedEventHandler MessageAdded;
        /// <summary>
        /// 是否异步处理
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// 日志文件的保存路径
        /// </summary>
        public string LogFilePath { get; private set; }
        /// <summary>
        /// 日志锁
        /// </summary>
        private readonly object loggerLock = new object();


        public Logger(string logFilePath)
        {
            this.LogFilePath = logFilePath;
            this.Async = true;
        }

        /// <summary>
        /// 增加日志信息
        /// </summary>
        /// <param name="message"></param>
        protected void OnMessageAdded(string message)
        {
            if (MessageAdded != null)
            {
                MessageAdded(message);
            }
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        public void WriteLine(string message)
        {
            if (!string.IsNullOrEmpty(this.LogFilePath) && !Directory.Exists(this.LogFilePath))
            {
                Directory.CreateDirectory(this.LogFilePath);
            }

            if (!string.IsNullOrEmpty(message))
            {
                if (Async)
                {
                    Task.Factory.StartNew(() => WriteLineInternal(message));
                    Task.Factory.StartNew(() => CheckExpireFile());
                }
                else
                {
                    WriteLineInternal(message);
                    CheckExpireFile();
                }
            }
        }

        private void CheckExpireFile()
        {
            string[] logFiles = Directory.GetFiles(this.LogFilePath);

            if (logFiles != null && logFiles.Length > 0)
            {
                foreach (string logFile in logFiles)
                {
                    try
                    {

                        DateTime logDateTime = Convert.ToDateTime(Path.GetFileName(logFile).Replace("HydraCss-", string.Empty).Replace(".txt", string.Empty));

                        if (DateTime.Now.Subtract(logDateTime).TotalDays > 3)
                        {
                            File.Delete(logFile);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        private void WriteLineInternal(string message)
        {
            lock (loggerLock)
            {
                try
                {
                    string logFileName = Path.Combine(this.LogFilePath, string.Format("HydraCss-{0:yyyy-MM-dd}.txt", DateTime.Now));

                    int index = 0;

                    while (this.IsFileLocked(logFileName))
                    {
                        logFileName = Path.Combine(this.LogFilePath, string.Format("{0}-{1}.txt", Path.GetFileNameWithoutExtension(logFileName), ++index));
                    }

                    message = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} - {message}";

                    if (!File.Exists(logFileName))
                    {
                        using (StreamWriter sw = new StreamWriter(File.Create(logFileName), Encoding.UTF8))
                        {
                            sw.WriteLine(message);
                            sw.Flush();
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(logFileName, true, Encoding.UTF8))
                        {
                            sw.WriteLine(message);
                            sw.Flush();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public bool IsFileLocked(string filename)
        {
            bool Locked = false;
            try
            {
                FileStream fs = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                fs.Close();
            }
            catch (IOException ex)
            {
                Locked = true;
            }
            return Locked;
        }

        /// <summary>
        /// 根据格式写日志
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }
        /// <summary>
        ///  根据异常字符串记录日志
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public void WriteException(string exception, string message = "Exception")
        {
            WriteLine("{0}:{1}{2}", message, Environment.NewLine, exception);
        }

        /// <summary>
        /// 根据异常对象记录日志
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        public void WriteException(Exception exception, string message = "Exception")
        {
            WriteException(exception.ToString(), message);
        }
    }
}
