using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Helpers
{
    /// <summary>
    /// 必备数据的帮助类
    /// </summary>
    public class EssentialHelper
    {
        private static long MemoryBufferCount = 0;
        private static Dictionary<string, string> iEssentials = new Dictionary<string, string>();
        private static string EssentialFullName;

        /// <summary>
        /// 初始必务备数据的所在文件
        /// </summary>
        /// <param name="essentialFullName"></param>
        public static void Init(string essentialFullName)
        {
            EssentialFullName = essentialFullName;
        }

        /// <summary>
        /// 增加必备数据
        /// </summary>
        /// <param name="essentials"></param>
        public static void AddRange(Dictionary<string, string> essentials)
        {
            if (CheckExist())
            {
                using (StreamWriter sw = new StreamWriter(EssentialFullName, true, Encoding.UTF8))
                {
                    foreach (string keyValue in essentials.Keys)
                    {
                        sw.WriteLine(string.Format("{0}={1}", keyValue, essentials[keyValue]));
                    }
                }
            }
        }

        /// <summary>
        /// 保存必要的数据文件
        /// </summary>
        public static void AddRangeAndClear(Dictionary<string, string> essentials)
        {
            if (CheckExist())
            {
                using (StreamWriter sw = new StreamWriter(EssentialFullName, false, Encoding.UTF8))
                {
                    foreach (string keyValue in essentials.Keys)
                    {
                        sw.WriteLine(string.Format("{0}={1}", keyValue, essentials[keyValue]));
                    }
                }
            }
        }

        /// <summary>
        /// 增加必备数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(string key, string value)
        {
            if (CheckExist())
            {
                using (StreamWriter sw = new StreamWriter(EssentialFullName, true, Encoding.UTF8))
                {
                    sw.WriteLine(string.Format("{0}={1}", key, value));
                }
            }
        }

        /// <summary>
        /// 获得所有的必备数据
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetAll()
        {
            if (CheckExist())
            {
                FileInfo fi = new FileInfo(EssentialFullName);

                if (MemoryBufferCount != fi.Length)
                {
                    Synchronization();

                    MemoryBufferCount = fi.Length;
                }

                Dictionary<string, string> duplicate = new Dictionary<string, string>();

                foreach (string keyName in iEssentials.Keys)
                {
                    duplicate.Add(keyName, iEssentials[keyName]);
                }

                return duplicate;
            }

            return null;
        }

        /// <summary>
        /// 根据KEY获得相应的必备数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key)
        {
            if (CheckExist())
            {
                FileInfo fi = new FileInfo(EssentialFullName);

                if (MemoryBufferCount != fi.Length)
                {
                    Synchronization();

                    MemoryBufferCount = fi.Length;
                }

                if (iEssentials.ContainsKey(key))
                    return iEssentials[key];
            }

            return string.Empty;
        }

        /// <summary>
        /// 同步文件和内存里面的必备数据
        /// </summary>
        private static void Synchronization()
        {
            using (StreamReader sr = new StreamReader(EssentialFullName, Encoding.UTF8))
            {
                string line = sr.ReadLine();

                while (!string.IsNullOrWhiteSpace(line))
                {
                    int splitIndex = line.IndexOf("=");

                    string keyName = line.Substring(0, splitIndex);
                    string value = line.Substring(splitIndex + 1);

                    if (iEssentials.ContainsKey(keyName))
                    {
                        iEssentials[keyName] = value;
                    }
                    else
                    {
                        iEssentials.Add(keyName, value);
                    }

                    line = sr.ReadLine();
                }
            }

        }

        /// <summary>
        /// 检查是否存在必备文件如果不存在，就创建
        /// </summary>
        /// <returns></returns>
        private static bool CheckExist()
        {
            while (!File.Exists(EssentialFullName))
            {
                File.Create(EssentialFullName).Close();
            }

            return true;
        }

        /// <summary>
        /// 检查是否存在必备文件
        /// </summary>
        /// <returns></returns>
        public static bool Exist()
        {
            return File.Exists(EssentialFullName);
        }
    }
}
