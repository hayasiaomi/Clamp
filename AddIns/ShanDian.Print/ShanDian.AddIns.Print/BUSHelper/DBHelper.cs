using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Services.BUSHelper
{
    public class DBHelper
    {
        #region init
        private DBHelper() { }

        private static DBHelper _instance;

        public static DBHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBHelper();
                }
                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// 获取数据库事务sql
        /// </summary>
        /// <param name="sb"></param>
        public void GetTransactionSql(StringBuilder sb)
        {
            sb.Insert(0, "BEGIN;");
            sb.Append("COMMIT");
        }

        /// <summary>
        /// 获取更新时间的字符串
        /// </summary>
        /// <param name="updateTime"></param>
        /// <returns></returns>
        public string GetUpdateTimeStr(DateTime updateTime)
        {
            return updateTime.ToString("yyyy-MM-dd HH:mm:ss.ffff");
        }
    }
}
