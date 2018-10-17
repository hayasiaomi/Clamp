using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hydra.Framework.Partial.Mqtt;
using Hydra.Framework.Services.HttpUtility;
using Hydra.Framework.SqlContent;
using Hydra.Parts.Currency.Information;
using ShanDian.AddIns.Print.Dto.Platform;
using ShanDian.AddIns.Print.Dto.Restaurant;
using ShanDian.AddIns.Print.Dto.ScanCode;
using ShanDian.AddIns.Print.Interface;
using ShanDian.AddIns.Print.Model;
using ShanDian.AddIns.Print.Services.BUSHelper;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.AddIns.Print.Services.Module
{
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
            _dbVersionList.Add("1.0.0.2");
            _dbVersionList.Add("1.0.0.3");
            //轻餐版数据库的版本
            _dbVersionList.Add("1.0.0.4");
            _dbVersionList.Add("1.0.0.5");
            _dbVersionList.Add("1.0.0.6");
            //海外国际版是1.0.0.7
            _dbVersionList.Add("1.0.0.8");
            _dbVersionList.Add("1.0.0.9");
            _dbVersionList.Add("1.0.0.10");
            _dbVersionList.Add("1.0.0.11");
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
                PrintLogUtility.Writer.SendFullError(ex);
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
                PrintLogUtility.Writer.SendInfo(ex.Message);
            }
            return dbHistoryList;
        }

        public void V1_0_0_1(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 创建tb_dbhistory
                //DROP TABLE IF EXISTS "main"."Machine";
                updateSql.Append("DROP TABLE IF EXISTS tb_dbhistory; ");
                updateSql.Append("CREATE TABLE tb_dbhistory ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("versionCode VARCHAR (30), ");
                updateSql.Append("updateTime  DATETIME ); ");

                #endregion


                #region 创建tb_voucherExpand
                //DROP TABLE IF EXISTS "main"."Machine";
                updateSql.Append("DROP TABLE IF EXISTS tb_voucherExpand; ");
                updateSql.Append("CREATE TABLE tb_voucherExpand ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("templateCode VARCHAR (2000), ");
                updateSql.Append("type  int , ");
                updateSql.Append("alignment  int , ");
                updateSql.Append("content VARCHAR (2000));  ");

                #endregion

                //#region 增加列tb_payinfo
                //var tableColumn = _repositoryContext.GetSet<TableColumn>("PRAGMA table_info(tb_payinfo)", null);
                //if (tableColumn != null && !tableColumn.Exists(t => t.Name == "invoiceTitle"))
                //{
                //    updateSql.Append("ALTER TABLE tb_payinfo ADD COLUMN invoiceTitle VARCHAR(500); ");
                //}
                //if (tableColumn != null && !tableColumn.Exists(t => t.Name == "taxpayerNo"))
                //{
                //    updateSql.Append("ALTER TABLE tb_payinfo ADD COLUMN taxpayerNo VARCHAR (100); ");
                //}
                //#endregion

                #region 记录更新
                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                #endregion

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_2(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 创建tb_voucherExpand
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "口碑预点单", "PRT_SO_1001", 1, 1, 1, 0, 1, "..\\PrintTemplate\\YDDD.html", 5);

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
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_3(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");


                #region 记录更新
                updateSql.AppendFormat("INSERT INTO tb_voucherExpand(TemplateCode,Type,Alignment,Content) VALUES ('{0}',{1},{2},'{3}'); ", "PRT_FI_0001", 10, 0, 0);
                updateSql.AppendFormat("INSERT INTO tb_voucherExpand(TemplateCode,Type,Alignment,Content) VALUES ('{0}',{1},{2},'{3}'); ", "PRT_FI_0001", 11, 0, 0);
                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "堂食汇总单", "PRT_FI_0004", 1, 1, 1, 1, 0, "..\\PrintTemplate\\TSHZD.html", 6);
                #endregion

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_4(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 创建tb_cashBoxSetInfo
                updateSql.Append("DROP TABLE IF EXISTS tb_cashBoxSetInfo; ");
                updateSql.Append("CREATE TABLE tb_cashBoxSetInfo ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("terminaId VARCHAR (100), ");
                updateSql.Append("terminalName VARCHAR (100), ");
                updateSql.Append("printId VARCHAR (100), ");
                updateSql.Append("printName VARCHAR (100), ");
                updateSql.Append("isSet int);  ");
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
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_5(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 记录更新
                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "自动核销失败", "PRT_SO_0002", 1, 1, 1, 0, 0, "..\\PrintTemplate\\ZDHXSB.html", 7);
                #endregion

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_6(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 记录更新
                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "善点轻餐营业汇总单", "PRT_FI_1001", 1, 1, 1, 0, 0, "..\\PrintTemplate\\QCYYHZ.html", 8);
                #endregion

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_8(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                //sort分类 10 的原因是国际版已经的结账单是 9 了
                #region 记录更新
                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "菜品统计单", "PRT_FI_0005", 1, 1, 1, 0, 0, "..\\PrintTemplate\\CPTJD.html", 10);
                #endregion

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_9(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 记录更新
                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "交班单", "PRT_FI_0006", 1, 1, 1, 0, 0, "..\\PrintTemplate\\QCJBD.html", 11);
                #endregion

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_10(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 记录更新
                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "外卖单", "PRT_TO_0001", 1, 1, 1, 0, 1, "..\\PrintTemplate\\JHWMD.html", 11);
                updateSql.AppendFormat("UPDATE tb_voucher SET GlobalVoucher = 0 WHERE TemplateCode = 'PRT_SO_1001' ;");
                #endregion

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public void V1_0_0_11(string version)
        {
            try
            {
                StringBuilder updateSql = new StringBuilder();
                updateSql.Append("PRAGMA foreign_keys = off; ");
                updateSql.Append("BEGIN TRANSACTION; ");

                #region 创建tb_printGroup
                updateSql.Append("DROP TABLE IF EXISTS tb_printGroup; ");
                updateSql.Append("CREATE TABLE tb_printGroup ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("name VARCHAR (100), ");
                updateSql.Append("printCode int, ");
                updateSql.Append("groupState int, ");
                updateSql.Append("createDate DATETIME, ");
                updateSql.Append("sort int );");
                #endregion

                #region 创建tb_printGroupScheme
                updateSql.Append("DROP TABLE IF EXISTS tb_printGroupScheme; ");
                updateSql.Append("CREATE TABLE tb_printGroupScheme ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("pcid VARCHAR (100), ");
                updateSql.Append("name VARCHAR (100), ");
                updateSql.Append("printId VARCHAR (100), ");
                updateSql.Append("isDefault int, ");
                updateSql.Append("state int, ");
                updateSql.Append("groupId int, ");
                updateSql.Append("modifyTime DATETIME DEFAULT (datetime('now', 'localtime')), ");
                updateSql.Append("createTime DATETIME DEFAULT (datetime('now', 'localtime')));  ");
                #endregion

                #region 创建tb_printSetInfo
                updateSql.Append("DROP TABLE IF EXISTS tb_printSetInfo; ");
                updateSql.Append("CREATE TABLE tb_printSetInfo ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("name VARCHAR (100), ");
                updateSql.Append("describe VARCHAR (100), ");
                updateSql.Append("key VARCHAR (100), ");
                updateSql.Append("value VARCHAR (100), ");
                updateSql.Append("range int, ");
                updateSql.Append("combineId  int ); ");
                #endregion

                #region 创建tb_schemeTable
                updateSql.Append("DROP TABLE IF EXISTS tb_schemeTable; ");
                updateSql.Append("CREATE TABLE tb_schemeTable ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("schemeId int, ");
                updateSql.Append("mkTableID VARCHAR (100), ");
                updateSql.Append("erpTableID VARCHAR (100), ");
                updateSql.Append("erpTableAreaID VARCHAR (100), ");
                updateSql.Append("tableName  VARCHAR (100) ); ");
                #endregion

                #region 创建tb_schemeDishType
                updateSql.Append("DROP TABLE IF EXISTS tb_schemeDishType; ");
                updateSql.Append("CREATE TABLE tb_schemeDishType ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("schemeId int, ");
                updateSql.Append("mkDishTypeID VARCHAR (100), ");
                updateSql.Append("erpDishTypeID VARCHAR (100), ");
                updateSql.Append("dishTypeName  VARCHAR (100) ); ");
                #endregion

                #region 创建tb_schemeVoucher
                updateSql.Append("DROP TABLE IF EXISTS tb_schemeVoucher; ");
                updateSql.Append("CREATE TABLE tb_schemeVoucher ( ");
                updateSql.Append("id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,");
                updateSql.Append("name VARCHAR (100), ");
                updateSql.Append("describe VARCHAR (100), ");
                updateSql.Append("voucherCode int, ");
                updateSql.Append("templateCode VARCHAR (100), ");
                updateSql.Append("isEnabled int, ");
                updateSql.Append("printNum int, ");
                updateSql.Append("pattern int, ");
                updateSql.Append("schemeId  int ); ");
                #endregion

                #region 更新打印方案组

                updateSql.AppendFormat("INSERT INTO tb_printGroup (name,printCode,groupState,createDate,sort) VALUES ('{0}','{1}',1, datetime('now', 'localtime'),1) ;", "点菜打印方案", "10");
                updateSql.AppendFormat("INSERT INTO tb_printGroup (name,printCode,groupState,createDate,sort) VALUES ('{0}','{1}',1, datetime('now', 'localtime'),2) ;", "支付打印方案", "20");
                updateSql.AppendFormat("INSERT INTO tb_printGroup (name,printCode,groupState,createDate,sort) VALUES ('{0}','{1}',1, datetime('now', 'localtime'),3) ;", "后厨打印方案", "30");
                updateSql.AppendFormat("INSERT INTO tb_printGroup (name,printCode,groupState,createDate,sort) VALUES ('{0}','{1}',1,datetime('now', 'localtime'),4) ;", "预点打印方案", "40");
                updateSql.AppendFormat("INSERT INTO tb_printGroup (name,printCode,groupState,createDate,sort) VALUES ('{0}','{1}',1,datetime('now', 'localtime'),5) ;", "外卖打印方案", "50");

                #endregion

                var tableColumn3 = _repositoryContext.GetSet<TableColumn>("PRAGMA table_info(tb_printconfig)", null);
                if (tableColumn3 != null && !tableColumn3.Exists(t => t.Name == "connBrand" && t.Type == "VARCHAR (50)"))
                {
                    //重命名
                    updateSql.Append("ALTER TABLE tb_printconfig RENAME TO tempPrintconfig_1; ");
                    updateSql.Append(@"CREATE TABLE tb_printconfig ( ");
                    updateSql.Append("id INTEGER PRIMARY KEY UNIQUE, ");
                    updateSql.Append("printId VARCHAR (50) NOT NULL,");
                    updateSql.Append("pcid VARCHAR (32) NOT NULL, ");
                    updateSql.Append("terminalName VARCHAR (50) , ");
                    updateSql.Append("printName VARCHAR (50) NOT NULL, ");
                    updateSql.Append("alias VARCHAR (50) , ");
                    updateSql.Append("connStyle INTEGER NOT NULL DEFAULT (1),");

                    //IP地址、品牌、端口
                    updateSql.Append("connAddress VARCHAR (50) , ");
                    updateSql.Append("connBrand VARCHAR (50) , ");
                    updateSql.Append("connPort VARCHAR (50) , ");

                    updateSql.Append("paperType INTEGER NOT NULL DEFAULT (0), ");
                    updateSql.Append("paperWidth Decimal(10,2) NOT NULL DEFAULT (40), ");
                    updateSql.Append("topMargin Decimal(10,2) NOT NULL DEFAULT (10), ");
                    updateSql.Append("leftMargin Decimal(10,2) NOT NULL DEFAULT (10),");
                    updateSql.Append("modifyTime DATETIME NOT NULL DEFAULT (datetime('now', 'localtime')) , ");
                    updateSql.Append("isDefault INTEGER NOT NULL ,");
                    updateSql.Append("updated INTEGER NOT NULL ,");
                    updateSql.Append("enable INTEGER NOT NULL ,");
                    updateSql.Append("state INTEGER NOT NULL );");


                    updateSql.Append(@"INSERT INTO tb_printconfig ( id, printId,  pcid, terminalName, printName, alias, connStyle, paperType, paperWidth, topMargin, leftMargin, modifyTime, isDefault, updated, enable, state )
                    SELECT id, printId,  pcid, terminalName, printName, alias, connStyle, paperType, paperWidth, topMargin, leftMargin, modifyTime, isDefault, updated, enable, state FROM tempPrintconfig_1;");
                    updateSql.Append("DROP TABLE tempPrintconfig_1; ");
                }

                updateSql.AppendFormat("INSERT INTO tb_dbhistory (versionCode,updateTime) VALUES ('{0}',datetime('now', 'localtime')); ", version);
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "厨打总单", "PRT_SO_3001", 1, 1, 1, 0, 0, "..\\PrintTemplate\\QCYYHZ.html", 12);
                updateSql.AppendFormat("INSERT INTO tb_voucher (VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Path,Sort) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'); ", "厨打分单", "PRT_SO_3002", 1, 1, 1, 0, 0, "..\\PrintTemplate\\QCYYHZ.html", 13);

                updateSql.Append("COMMIT TRANSACTION; ");
                updateSql.Append("PRAGMA foreign_keys = on; ");

                _repositoryContext.Execute(updateSql.ToString(), null);
            }
            catch (Exception ex)
            {
                DataBaseLog.Writer.SendInfo(ex.Message);
            }

            try
            {
                OtherEventV1_0_0_11(_repositoryContext);
                TKOrderEventV1_0_0_11(_repositoryContext);
            }
            catch (Exception ex)
            {
                DataBaseLog.Writer.SendInfo($"[数据库数据迁移]V1.0.0.7版本迁移异常：{ex.ToString()}");
            }
        }

        #region V1_0_0_11版本额外事件

        private void OtherEventV1_0_0_11(IRepositoryContext repositoryContext)
        {
            //至迁移扫码点菜单和支付凭证
            var voucherList = repositoryContext.GetSet<Voucher>("select * from tb_voucher where TemplateCode=@TemplateCode1 or TemplateCode=@TemplateCode2", new { TemplateCode1 = "PRT_SO_0001", TemplateCode2 = "PRT_FI_0001" });
            if (voucherList == null || voucherList.Count <= 0)
            {
                //没有单据可以迁移
                return;
            }

            var printSchemeList = repositoryContext.GetSet<PrintScheme>("select * from tb_printScheme where LocalMachine=@LocalMachine And PrintNum>@PrintNum", new { LocalMachine = 2, PrintNum = 0 });
            if (printSchemeList == null || printSchemeList.Count <= 0)
            {
                //没有方案需要迁移
                return;
            }

            //获取餐桌信息
            List<TableInfo> tableInfos = null;
            var scanCodeServices = MLandLoader.Load<IScanCodeServices>(Global.PartName);
            var tableInfo = scanCodeServices.GetTableInfo();
            if (scanCodeServices.Flag)
            {
                if (tableInfo.tableInfos != null && tableInfo.tableInfos.Count > 0)
                {
                    tableInfos = tableInfo.tableInfos;
                }
            }
            StringBuilder sqlSb = new StringBuilder();

            //100 非对接
            //200 对接
            //300 轻餐
            PrintSetInfo newPrintSetInfo = new PrintSetInfo()
            {
                Name = "对接信息",
                Key = "100",
                Range = 0,
                CombineId = 0
            };
            RestaurantsInfo restInfo = BusinessHelper.Instance.GetRestaurantsInfo(0);
            if (restInfo != null)
            {
                if (!restInfo.IsOpenQC)
                {
                    //凭证存在
                    Voucher v_ScanCode = voucherList.FirstOrDefault(t => t.TemplateCode == "PRT_SO_0001");
                    if (v_ScanCode != null)
                    {
                        InsertNewSchemeByVoucher(repositoryContext, sqlSb, v_ScanCode, 10, printSchemeList, tableInfos);
                    }
                    newPrintSetInfo.Value = "200";
                }
                else
                {
                    newPrintSetInfo.Value = "300";
                    DataBaseLog.Writer.SendInfo("[数据库数据迁移]V1.0.0.11版本：轻餐系统不迁移点菜打印方案！");
                }
            }
            else
            {
                DataBaseLog.Writer.SendInfo("[数据库数据迁移]V1.0.0.11版本：获取餐厅信息异常，点菜打印方案迁移失败！");
            }
            GetInsertTB_printSetInfoSql(sqlSb, newPrintSetInfo);//新增对接配置的
            Voucher v_PayProof = voucherList.FirstOrDefault(t => t.TemplateCode == "PRT_FI_0001");
            if (v_PayProof != null)
            {
                InsertNewSchemeByVoucher(repositoryContext, sqlSb, v_PayProof, 20, printSchemeList, tableInfos);
            }

            if (sqlSb.Length > 0)
            {
                try
                {
                    DBHelper.Instance.GetTransactionSql(sqlSb);
                    repositoryContext.Execute(sqlSb.ToString(), null);
                }
                catch (Exception ex)
                {
                    DataBaseLog.Writer.SendInfo($"[数据库数据迁移]V1.0.0.11版本迁移异常1：{ex.ToString()}，插入数据：" + sqlSb);
                }
            }

            #region 添加后厨打印默认的全局变量
            var printGroup = repositoryContext.FirstOrDefault<PrintGroup>("select * from tb_printGroup where printCode = 30", new { });
            if (printGroup != null)
            {
                PrintSetInfo printSetInfo = new PrintSetInfo()
                {
                    Name = "套餐按子菜品分单打印",
                    Key = "103",
                    Value = "true",
                    Range = 1,
                    CombineId = printGroup.Id
                };

                var voucherResult = repositoryContext.Execute("insert into tb_printSetInfo (name,key,value,range,combineId)VALUES(@name,@key,@value,@range,@combineId)", printSetInfo);
                if (voucherResult <= 0)
                {
                    DataBaseLog.Writer.SendInfo($"[数据库数据迁移]V1.0.0.11版本迁移异常2：初始化套餐按子菜品分单打印");
                }
            }
            #endregion
        }

        private void TKOrderEventV1_0_0_11(IRepositoryContext repositoryContext)
        {
            //迁移外卖打印
            var voucherTkOut = repositoryContext.FirstOrDefault<Voucher>("select * from tb_voucher where TemplateCode=@TemplateCode1 ", new { TemplateCode1 = "PRT_TO_0001" });
            if (voucherTkOut == null)
            {
                //没有方案需要迁移
                return;
            }

            var printSchemeList = repositoryContext.GetSet<PrintScheme>("select * from tb_printScheme where LocalMachine=@LocalMachine And PrintNum>@PrintNum And VoucherId = @VoucherId", new { LocalMachine = 2, PrintNum = 0, VoucherId = voucherTkOut.Id });
            if (printSchemeList == null || printSchemeList.Count <= 0)
            {
                //没有方案需要迁移
                return;
            }

            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = '50' ", new { });
            if (printGroup != null)
            {
                bool defaultFlag = true;
                foreach (var item in printSchemeList)
                {
                    var newPrintGroupScheme = new PrintGroupScheme()
                    {
                        Pcid = item.PcId,
                        Name = item.Name,
                        PrintId = item.PrintId,
                        IsDefault = defaultFlag ? 1 : 0,
                        State = 1,
                        GroupId = printGroup.Id,
                    };

                    //创建新的打印小方案
                    int insertResult = repositoryContext.Execute("insert into tb_printGroupScheme(Pcid,Name,PrintId,IsDefault,State,GroupId,CreateTime,ModifyTime)VALUES(@Pcid,@Name,@PrintId,@IsDefault,@State,@GroupId,datetime('now', 'localtime'),datetime('now', 'localtime'));", newPrintGroupScheme);

                    if (insertResult <= 0)
                    {
                        DataBaseLog.Writer.SendInfo($"[数据库数据迁移]V1.0.0.11版本迁移异常3：外卖迁移打印方案1");
                        continue;
                    }

                    var printGroupSchemeTemp = repositoryContext.FirstOrDefault<PrintGroupScheme>("select * from tb_printGroupScheme where groupId = @groupId order by createTime desc ,id desc", new { groupId = newPrintGroupScheme.GroupId });
                    if (printGroupSchemeTemp == null)
                    {
                        DataBaseLog.Writer.SendInfo($"[数据库数据迁移]V1.0.0.11版本迁移异常3：外卖迁移打印方案2");
                        continue;
                    }
                    SchemeVoucher newSchemeVoucher = new SchemeVoucher()
                    {
                        Name = "外卖单",
                        Describe = "",
                        VoucherCode = 50,
                        IsEnabled = 1,
                        PrintNum = item.PrintNum,
                        Pattern = 2,//点菜打印方案默认对接
                        TemplateCode = "PRT_TO_0001",
                        SchemeId = printGroupSchemeTemp.Id
                    };
                    var insertStr = repositoryContext.Execute($"insert into tb_schemeVoucher(Name,Describe,VoucherCode,TemplateCode,IsEnabled,PrintNum,Pattern,SchemeId)VALUES('{newSchemeVoucher.Name}','{newSchemeVoucher.Describe}',{newSchemeVoucher.VoucherCode},'{newSchemeVoucher.TemplateCode}',{newSchemeVoucher.IsEnabled},{newSchemeVoucher.PrintNum},{newSchemeVoucher.Pattern},{newSchemeVoucher.SchemeId});", new { });
                    defaultFlag = false;

                    if (insertStr <= 0)
                    {
                        DataBaseLog.Writer.SendInfo($"[数据库数据迁移]V1.0.0.11版本迁移异常3：外卖迁移打印方案3");
                        continue;
                    }
                }
            }
        }

        private void InsertNewSchemeByVoucher(IRepositoryContext repositoryContext, StringBuilder sqlSb, Voucher voucher, int businessType, List<PrintScheme> printSchemeList, List<TableInfo> tableInfos)
        {
            //根据单据ID找到使用该单据的打印方案（旧）
            var printScheme_ScanCodeList = printSchemeList.Where(t => t.VoucherId == voucher.Id).ToList();
            if (printScheme_ScanCodeList != null && printScheme_ScanCodeList.Count > 0)
            {
                //找到新的大方案
                var printGroupList = repositoryContext.GetSet<PrintGroup>("select * from tb_printGroup where PrintCode=@PrintCode", new { PrintCode = businessType });
                if (printGroupList != null && printGroupList.Count > 0)
                {
                    var printGroupTemp = printGroupList.OrderBy(t => t.CreateDate).FirstOrDefault();
                    bool defaultFlag = true;
                    foreach (var printScheme in printScheme_ScanCodeList)
                    {
                        var newPrintGroupScheme = new PrintGroupScheme()
                        {
                            Pcid = printScheme.PcId,
                            Name = printScheme.Name,
                            PrintId = printScheme.PrintId,
                            IsDefault = defaultFlag ? 1 : 0,
                            State = 1,
                            GroupId = printGroupTemp.Id,
                        };

                        //创建新的打印小方案
                        int insertResult = repositoryContext.Execute("insert into tb_printGroupScheme(Pcid,Name,PrintId,IsDefault,State,GroupId,CreateTime,ModifyTime)VALUES(@Pcid,@Name,@PrintId,@IsDefault,@State,@GroupId,datetime('now', 'localtime'),datetime('now', 'localtime'));",
                               newPrintGroupScheme);
                        if (insertResult > 0)
                        {
                            var printGroupSchemeList = repositoryContext.GetSet<PrintGroupScheme>("select Id from tb_printGroupScheme where Pcid=@Pcid and PrintId=@PrintId and GroupId=@GroupId order by CreateTime desc",
                                     new { Pcid = newPrintGroupScheme.Pcid, PrintId = newPrintGroupScheme.PrintId, GroupId = newPrintGroupScheme.GroupId });
                            if (printGroupSchemeList != null && printGroupSchemeList.Count > 0)
                            {
                                //找到添加的小方案
                                var printGroupSchemeTemp = printGroupSchemeList.FirstOrDefault();

                                #region 迁移餐桌数据

                                //创建成功寻找旧区域打印方案，存在则取出进行迁移
                                List<PrintSchemeLabel> printSchemeLabelList = repositoryContext.GetSet<PrintSchemeLabel>("select * from tb_printSchemeLabel where PrintSchemeId=@PrintSchemeId",
                                    new { PrintSchemeId = printScheme.Id });
                                if (printSchemeLabelList != null && printSchemeLabelList.Count >= 0 && tableInfos != null && tableInfos.Count > 0)//没有获取到餐桌数据不迁移
                                {
                                    //迁移餐桌数据
                                    foreach (var printSchemeLabel in printSchemeLabelList)
                                    {
                                        //找对接组件获取餐桌信息，迁移的时候正好补全数据库的餐桌信息
                                        var tableInfoTemp = tableInfos.FirstOrDefault(t => t.MKTableID == printSchemeLabel.LabelId);
                                        if (tableInfoTemp != null)
                                        {
                                            sqlSb.Append($"insert into tb_schemeTable(SchemeId,MKTableID,ErpTableID,ErpTableAreaID,TableName)VALUES({printGroupSchemeTemp.Id},'{tableInfoTemp.MKTableID}','{tableInfoTemp.ErpTableID}','{tableInfoTemp.ErpAreaID}','{tableInfoTemp.Name}');");
                                        }
                                    }
                                }

                                #endregion

                                if (businessType == 10)
                                {
                                    InsertNewSchemeVoucher_Order(sqlSb, printScheme, printGroupSchemeTemp);
                                }
                                else
                                {
                                    InsertNewSchemeVoucher_Pay(sqlSb, printScheme, printGroupSchemeTemp);
                                }
                            }
                        }

                        defaultFlag = false;
                    }
                }
            }
        }

        /// <summary>
        /// 添加小方案对应的单据（例如：“点菜打印方案”包括“点菜单”和“结账单”）
        /// </summary>
        private void InsertNewSchemeVoucher_Order(StringBuilder sqlSb, PrintScheme printScheme, PrintGroupScheme printGroupSchemeTemp)
        {
            #region “点菜打印方案”包括“点菜单”和“结账单”

            //退款凭证引用支付凭证的设置
            SchemeVoucher newSchemeVoucher10 = new SchemeVoucher()
            {
                Name = "点菜单",
                VoucherCode = 10,
                IsEnabled = 1,
                PrintNum = printScheme.PrintNum,
                Pattern = 2,//点菜打印方案默认对接
                TemplateCode = "PRT_SO_0001",
                SchemeId = printGroupSchemeTemp.Id
            };
            SchemeVoucher newSchemeVoucher11 = new SchemeVoucher()
            {
                Name = "结账单",
                VoucherCode = 11,
                IsEnabled = 1,
                PrintNum = printScheme.PrintNum,
                Pattern = 2,//点菜打印方案默认对接
                TemplateCode = "PRT_SO_0001",
                SchemeId = printGroupSchemeTemp.Id
            };
            GetInsertTB_printGroupSchemeSql(sqlSb, newSchemeVoucher10);
            GetInsertTB_printGroupSchemeSql(sqlSb, newSchemeVoucher11);

            #endregion

            #region 点菜打印方案全局方案

            PrintSetInfo newPrintSetInfo101 = new PrintSetInfo()
            {
                Name = "下单失败打印《下单失败》",
                Describe = "《下单失败》",
                Key = "101",
                Value = "true",
                Range = 2,
                CombineId = printGroupSchemeTemp.Id
            };

            PrintSetInfo newPrintSetInfo102 = new PrintSetInfo()
            {
                Name = "结账失败打印《核销失败》",
                Describe = "《核销失败》",
                Key = "102",
                Value = "true",
                Range = 2,
                CombineId = printGroupSchemeTemp.Id
            };

            GetInsertTB_printSetInfoSql(sqlSb, newPrintSetInfo101);
            GetInsertTB_printSetInfoSql(sqlSb, newPrintSetInfo102);

            #endregion
        }

        /// <summary>
        /// 添加小方案对应的单据（例如：“支付打印方案”包括“支付凭证”和“退款凭证”）
        /// </summary>
        private void InsertNewSchemeVoucher_Pay(StringBuilder sqlSb, PrintScheme printScheme, PrintGroupScheme printGroupSchemeTemp)
        {
            #region “支付打印方案”包括“支付凭证”和“退款凭证”

            //退款凭证引用支付凭证的设置
            SchemeVoucher newSchemeVoucher20 = new SchemeVoucher()
            {
                Name = "支付凭证",
                VoucherCode = 20,
                IsEnabled = 1,
                PrintNum = printScheme.PrintNum,
                Pattern = 0,//点菜打印方案默认对接
                TemplateCode = "PRT_FI_0001",
                SchemeId = printGroupSchemeTemp.Id
            };
            SchemeVoucher newSchemeVoucher21 = new SchemeVoucher()
            {
                Name = "退款凭证",
                VoucherCode = 21,
                IsEnabled = 1,
                PrintNum = printScheme.PrintNum,
                Pattern = 0,//点菜打印方案默认对接
                TemplateCode = "PRT_FI_0002",
                SchemeId = printGroupSchemeTemp.Id
            };
            GetInsertTB_printGroupSchemeSql(sqlSb, newSchemeVoucher20);
            GetInsertTB_printGroupSchemeSql(sqlSb, newSchemeVoucher21);

            #endregion
        }

        private void GetInsertTB_printGroupSchemeSql(StringBuilder sqlSb, SchemeVoucher newSchemeVoucher)
        {
            sqlSb.Append($"insert into tb_schemeVoucher(Name,Describe,VoucherCode,TemplateCode,IsEnabled,PrintNum,Pattern,SchemeId)VALUES('{newSchemeVoucher.Name}','{newSchemeVoucher.Describe}',{newSchemeVoucher.VoucherCode},'{newSchemeVoucher.TemplateCode}',{newSchemeVoucher.IsEnabled},{newSchemeVoucher.PrintNum},{newSchemeVoucher.Pattern},{newSchemeVoucher.SchemeId});");
        }

        private void GetInsertTB_printSetInfoSql(StringBuilder sqlSb, PrintSetInfo newPrintSetInfo)
        {
            sqlSb.Append($"insert into tb_printSetInfo(Name,Describe,Key,Value,Range,CombineId)VALUES('{newPrintSetInfo.Name}','{newPrintSetInfo.Describe}','{newPrintSetInfo.Key}','{newPrintSetInfo.Value}',{newPrintSetInfo.Range},{newPrintSetInfo.CombineId});");
        }

        #endregion

        #region 注册15881需要处理的请求(2018年8月13日)
        /// <summary>
        /// IStandardVerSiteService
        /// </summary>
        public static void LoadStandardVerSiteMethod()
        {
            MethodMap.AddItem(new MethodItem
            {
                PartName = Global.PartName,
                HttpMethod = HttpMethod.Put,
                Method = "ThirdPartPrint",
                ParamName = "thirdPartPrint",
                Url = "api/1.0.0.0/StandardVerSite/Print/ThirdPart",
            });

            #region 第三方打印V2

            MethodMap.AddItem(new MethodItem
            {
                PartName = Global.PartName,
                HttpMethod = HttpMethod.Put,
                Method = "ThirdPartPrintV2",
                ParamName = "thirdPartPrint",
                Url = "api/1.0.0.0/StandardVerSite/Print/ThirdPartV2",
            });

            MethodMap.AddItem(new MethodItem
            {
                PartName = Global.PartName,
                HttpMethod = HttpMethod.Get,
                Method = "ThirdPartPrintConfigList",
                ParamName = "printGetInfo",
                Url = "api/1.0.0.0/StandardVerSite/Print/ThirdPartV2/PrintConfigList",
            });


            #endregion
        }
        #endregion
    }
}
