using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace ShanDain.AIM.Helper
{
    public class MLog
    {
        public static event Action<string> Notice;
        private static MLog _instance = new MLog();
        public static MLog GetInstance()
        {
            return _instance;
        }

        private ILogger logger;

        public MLog()
        {
            logger = LogManager.GetLogger("LogCustom");
        }

        public void SendDebug(string message)
        {
            try
            {
                logger.Debug(message);
                Notice?.Invoke(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SendTrace(string message)
        {
            try
            {
                logger.Trace(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
