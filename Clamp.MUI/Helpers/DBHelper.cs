using Clamp.MUI.Biz;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Helpers
{
    /// <summary>
    /// 用于当前主框的数据存取
    /// </summary>
    public class DBHelper
    {
        public static DataHandler DBDataHandler = new DataHandler(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage.db"), "LocalPrinterInfo");

        static DBHelper()
        {
            //检测是否是第一次初始化
            if (!DBDataHandler.Exist())
            {
                DBDataHandler.Insert("FloatSwitch", true);
            }
        }

        public static void Store(string key, object value)
        {
            object dbResult = DBDataHandler.Get(key);

            if (dbResult == null)
            {
                DBDataHandler.Insert(key, value);
            }
            else
            {
                DBDataHandler.Update(key, value);
            }
        }

        public static DictionaryInfo Acquire(string key)
        {
            return DBDataHandler.Get(key);
        }

        public static object AcquireValue(string key)
        {
            return DBDataHandler.GetValue(key);
        }
    }


}
