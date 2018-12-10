using Clamp.MUI.Biz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Helpers
{
    /// <summary>
    /// 用于前端的数据存取
    /// </summary>
    internal class CDBHelper
    {
        public static DataHandler ChromiunDataHandler = new DataHandler(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chromiun.db"), "LocalChromiunInfo");

        public static void Add(string key, string value)
        {
            ChromiunDataHandler.Insert(key, value);
        }

        public static void Remove(string key)
        {
            ChromiunDataHandler.Delete(key);
        }

        public static void Modify(string key, string value)
        {
            ChromiunDataHandler.Update(key, value);
        }

        public static string Get(string key)
        {
            object value = ChromiunDataHandler.GetValue(key);

            if (value != null)
                return Convert.ToString(value);
            return string.Empty;
        }

        public static bool Exist(string key)
        {
            return ChromiunDataHandler.Exist(key);
        }

    }
}
