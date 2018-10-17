using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.SqlContent;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;
using ShanDian.AddIns.Print.Model;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.AddIns.Print.Services.BUSHelper
{
    public class PrintConfigHelper
    {
        private PrintConfigHelper() { }

        private static PrintConfigHelper _instance;
        public List<PrintSetInfo> PrintSetInfoList;
        private IRepositoryContext _repositoryContext;
        public static PrintConfigHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PrintConfigHelper();
                    _instance._repositoryContext = Global.RepositoryContext();
                    _instance.PrintSetInfoList = new List<PrintSetInfo>();
                    _instance.PrintSetInfoList = _instance.GetPrintSetInfoes();
                }
                return _instance;
            }
        }

        private List<PrintSetInfo> GetPrintSetInfoes()
        {
            List<PrintSetInfoDto> result = new List<PrintSetInfoDto>();
            var printSetInfos = _repositoryContext.GetSet<PrintSetInfo>("select id,name,key,value,range,combineId from tb_printSetInfo where range = @range order by id asc", new { range = 1 });
            if (printSetInfos != null && printSetInfos.Count > 0)
            {
                foreach (var info in printSetInfos)
                {
                    PrintSetInfoDto voucher = new PrintSetInfoDto();
                    voucher.Id = info.Id;
                    voucher.Name = info.Name;
                    voucher.Describe = info.Describe;
                    voucher.Key = info.Key;
                    voucher.Value = info.Value;
                    voucher.Range = info.Range;
                    voucher.CombineId = info.CombineId;

                    result.Add(voucher);
                }
            }

            return null;
        }

        /// <summary>
        /// 获取单据
        /// </summary>
        /// <param name="printGroupScheme"></param>
        /// <returns></returns>
        public List<SchemeVoucherDto> GetSchemeVoucherList(PrintGroupScheme printGroupScheme)
        {
            var schemeVoucher = _repositoryContext.GetSet<SchemeVoucher>("select id,name,voucherCode,templateCode,isEnabled,printNum,pattern,schemeId from tb_schemeVoucher t WHERE t.schemeId = @schemeId", new { schemeId = printGroupScheme.Id });

            if (schemeVoucher != null && schemeVoucher.Count > 0)
            {
                List<SchemeVoucherDto> schemeVoucherList = new List<SchemeVoucherDto>();

                foreach (var item in schemeVoucher)
                {
                    SchemeVoucherDto voucherItem = new SchemeVoucherDto();
                    voucherItem.Id = item.Id;
                    voucherItem.Name = item.Name;
                    voucherItem.Describe = item.Describe;
                    voucherItem.VoucherCode = item.VoucherCode;
                    voucherItem.TemplateCode = item.TemplateCode;
                    voucherItem.IsEnabled = item.IsEnabled;
                    voucherItem.PrintNum = item.PrintNum;
                    voucherItem.Pattern = item.Pattern;
                    voucherItem.SchemeId = item.SchemeId;

                    schemeVoucherList.Add(voucherItem);
                }

                return schemeVoucherList;
            }
            else
            {
                return new List<SchemeVoucherDto>();
            }
        }

        /// <summary>
        /// 获取打印方案配置
        /// </summary>
        /// <param name="printGroupScheme"></param>
        /// <returns></returns>
        public List<PrintSetInfoDto> GetSchemeSetInfoList(PrintGroupScheme printGroupScheme)
        {
            var schemeVoucher = _repositoryContext.GetSet<PrintSetInfo>("select id,name,key,value,range,combineId from tb_printSetInfo t WHERE t.combineId = @combineId", new { combineId = printGroupScheme.Id });

            if (schemeVoucher != null && schemeVoucher.Count > 0)
            {
                List<PrintSetInfoDto> printSetInfoList = new List<PrintSetInfoDto>();

                foreach (var item in schemeVoucher)
                {
                    PrintSetInfoDto voucherItem = new PrintSetInfoDto();
                    voucherItem.Id = item.Id;
                    voucherItem.Name = item.Name;
                    voucherItem.Describe = item.Describe;
                    voucherItem.Key = item.Key;
                    voucherItem.Value = item.Value;
                    voucherItem.Range = item.Range;
                    voucherItem.CombineId = item.CombineId;

                    printSetInfoList.Add(voucherItem);
                }

                return printSetInfoList;
            }
            else
            {
                return new List<PrintSetInfoDto>();
            }
        }

        /// <summary>
        /// 获取打印方案的菜品分类
        /// </summary>
        /// <param name="printGroupScheme"></param>
        /// <returns></returns>
        public List<SchemeDishTypeDto> GetSchemeDishTypeList(PrintGroupScheme printGroupScheme)
        {

            var schemeVoucher = _repositoryContext.GetSet<SchemeDishType>("select id,schemeId,mkDishTypeID,erpDishTypeID,dishTypeName from tb_schemeDishType t WHERE t.schemeId = @schemeId", new { schemeId = printGroupScheme.Id });

            if (schemeVoucher != null && schemeVoucher.Count > 0)
            {
                List<SchemeDishTypeDto> printDishTypes = new List<SchemeDishTypeDto>();

                foreach (var item in schemeVoucher)
                {
                    SchemeDishTypeDto dishType = new SchemeDishTypeDto();
                    dishType.Id = item.Id;
                    dishType.SchemeId = item.SchemeId;
                    dishType.MKDishTypeID = item.MKDishTypeID;
                    dishType.ErpDishTypeID = item.ErpDishTypeID;
                    dishType.DishTypeName = item.DishTypeName;

                    printDishTypes.Add(dishType);
                }

                return printDishTypes;
            }
            else
            {
                return new List<SchemeDishTypeDto>();
            }
        }

        /// <summary>
        /// 获取餐桌
        /// </summary>
        /// <param name="printGroupScheme"></param>
        /// <returns></returns>
        public List<SchemeTableDto> GetSchemeTableList(PrintGroupScheme printGroupScheme)
        {
            var schemeVoucher = _repositoryContext.GetSet<SchemeTable>("select id,schemeId,mkTableID,erpTableID,erpTableAreaID,tableName from tb_schemeTable t WHERE t.schemeId = @schemeId", new { schemeId = printGroupScheme.Id });

            if (schemeVoucher != null && schemeVoucher.Count > 0)
            {
                List<SchemeTableDto> schemeTableList = new List<SchemeTableDto>();

                foreach (var item in schemeVoucher)
                {
                    SchemeTableDto voucherItem = new SchemeTableDto();
                    voucherItem.Id = item.Id;
                    voucherItem.SchemeId = item.SchemeId;
                    voucherItem.MKTableID = item.MKTableID;
                    voucherItem.ErpTableID = item.ErpTableID;
                    voucherItem.ErpTableAreaID = item.ErpTableAreaID;
                    voucherItem.TableName = item.TableName;
                    voucherItem.SchemeId = item.SchemeId;

                    schemeTableList.Add(voucherItem);
                }

                return schemeTableList;
            }
            else
            {
                return new List<SchemeTableDto>();
            }
        }
    }
}
