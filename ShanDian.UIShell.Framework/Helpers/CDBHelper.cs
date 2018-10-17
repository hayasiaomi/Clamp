using ShanDian.UIShell.Framework.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Helpers
{
    /// <summary>
    /// 用于前端的数据存取
    /// </summary>
    public class CDBHelper
    {
        public readonly static ChromiunDataHandler ChromiunDataHandler = new ChromiunDataHandler();

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
