using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using ShanDian.LSRestaurant.Model.Dishes;
using ShanDian.LSRestaurant.ViewModel;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.LSRestaurant.Services
{
    /// <summary>
    /// DB的升级更新
    /// </summary>
    public class DatabaseServices
    {
        private readonly IRepositoryContext _repositoryContext;

        //升级版本号
        private readonly List<string> _dbVersionList;

        public DatabaseServices()
        {
            _repositoryContext = Global.RepositoryContext();
            _dbVersionList = new List<string>();
            _dbVersionList.Add("1.0.0.1");
            //_dbVersionList.Add("1.0.0.2");
        }

        /// <summary>
        /// 初始化更新数据库版本
        /// </summary>
        public static void InitDatabase()
        {
            try
            {
                Type tp = typeof(DatabaseServices);
                DatabaseServices databaseServices = new DatabaseServices();
                databaseServices.V1_0_0_0();
                string curVersion = databaseServices.GetComponentVersion();
                var dbHistoryList = databaseServices.GetDbHistory();
                if (dbHistoryList != null && dbHistoryList.Exists(t => t.VersionCode == curVersion))
                {
                    return;
                }
                foreach (string item in databaseServices._dbVersionList)
                {
                    if (!dbHistoryList.Exists(t => t.VersionCode == item))
                    {
                        MethodInfo mi = tp.GetMethod("V" + item.Replace(".", "_"));//得到方法“V1_0_0_1()”函数的信息
                        object res = mi.Invoke(databaseServices, new object[] { item });//
                    }
                }
            }
            catch (Exception ex)
            {
                //DishesLogUtility.Writer.SendFullError(ex);
            }

        }

        /// <summary>
        /// 获取组件版本
        /// </summary>
        /// <returns></returns>
        public string GetComponentVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// 获取DB版本升级信息
        /// </summary>
        /// <returns></returns>
        public List<DbHistory> GetDbHistory()
        {
            List<DbHistory> dbHistoryList = new List<DbHistory>();
            try
            {
                dbHistoryList = _repositoryContext.GetSet<DbHistory>("select id,versionCode,updateTime from tb_dbhistory", null);
            }
            catch (Exception ex)
            {
                //DishesLogUtility.Writer.SendInfo(ex.Message);
            }
            return dbHistoryList;
        }

        /// <summary>
        /// db初始化
        /// </summary>
        public void V1_0_0_0()
        {
            try
            {
                var sqliteMasterList = _repositoryContext.GetSet<SqliteMaster>("select * from sqlite_master where [type]='table'", null);
                if (sqliteMasterList == null || sqliteMasterList.Count < 1)//db初始化判断
                {
                    //
                    Assembly assembly = GetType().Assembly;
                    System.IO.Stream streamSmall = assembly.GetManifestResourceStream("ShanDian.LSRestaurant.Sql.Dishes.sql");
                    StreamReader sr = new StreamReader(streamSmall, Encoding.UTF8);
                    string text = sr.ReadToEnd();
                    StringBuilder updateSql = new StringBuilder();
                    updateSql.AppendFormat(text);
                    _repositoryContext.Execute(updateSql.ToString(), null);
                }
            }
            catch (Exception ex)
            {
                //DishesLogUtility.Writer.SendFullError(ex);
            }

        }

        public void V1_0_0_1(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 创建tb_dbhistory
                var tableColumn0 = _repositoryContext.GetSet<TableColumn>("PRAGMA table_info(tb_dbhistory)", null);
                if (tableColumn0 == null || tableColumn0.Count < 1)
                {
                    updateSql.Append("DROP TABLE IF EXISTS tb_dbhistory; ");
                    updateSql.Append("CREATE TABLE tb_dbhistory ( ");
                    updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                    updateSql.Append("versionCode VARCHAR (30), ");
                    updateSql.Append("updateTime  DATETIME ); ");
                }

                #endregion

                #region 批量刷新数据：分类全部显示

                updateSql.Append("update tb_categorydish set isHidden=0; ");

                #endregion

                #region 记录更新
                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                #endregion

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                //DishesLogUtility.Writer.SendFullError(ex);
            }

        }

        public void V1_0_0_2(string version)
        {
            try
            {
                //StringBuilder updateSql = new StringBuilder();
                //updateSql.Append("PRAGMA foreign_keys = off; ");
                //updateSql.Append("BEGIN TRANSACTION; ");

                //#region 创建tb_paymentsyn
                ////DROP TABLE IF EXISTS "main"."Machine";
                //updateSql.Append("DROP TABLE IF EXISTS tb_paymentsyn; ");
                //updateSql.Append("CREATE TABLE tb_paymentsyn ( ");
                //updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, ");
                //updateSql.Append("state INTEGER NOT NULL DEFAULT(0), ");
                //updateSql.Append("startTime DATETIME,");
                //updateSql.Append("endTime DATETIME,");
                //updateSql.Append("updateTime DATETIME DEFAULT (datetime('now', 'localtime')) NOT NULL,");
                //updateSql.Append("pcid VARCHAR (50),");
                //updateSql.Append("operator VARCHAR (50),");
                //updateSql.Append("device VARCHAR (50),");
                //updateSql.Append("payCount INTEGER NOT NULL DEFAULT(0), ");
                //updateSql.Append("refundCount INTEGER NOT NULL DEFAULT(0), ");
                //updateSql.Append("payAllCount INTEGER NOT NULL DEFAULT(0), ");
                //updateSql.Append("refundAllCount NOT NULL DEFAULT(0) ); ");

                //#endregion

                //#region 记录更新
                //updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                //#endregion

                //updateSql.Append("COMMIT TRANSACTION; ");
                //updateSql.Append("PRAGMA foreign_keys = on; ");

                //_repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                //DishesLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_3(string version)
        {
        }


    }
}
