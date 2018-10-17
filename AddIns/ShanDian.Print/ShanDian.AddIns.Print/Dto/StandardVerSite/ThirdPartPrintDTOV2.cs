using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;

namespace ShanDian.AddIns.Print.Dto.StandardVerSite
{
    public class ThirdPartPrint.DtoV2
    {
        /// <summary>
        /// 打印信息
        /// </summary>
        public string PrintInfos { set; get; }
    }

    public class PrintGetInfoDtoV2
    {
        public string PrintCode { get; set; }
    }

    /// <summary>
    /// 解析第三方打印信息
    /// </summary>
    public class PrintInfoV2
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { set; get; }

        /// <summary>
        /// 模板对应的数据（Json）
        /// </summary>
        public string TemplateData { set; get; }

        /// <summary>
        /// 打印方案集合
        /// </summary>
        public List<ErpPrintConfigDtoV2> PrintConfigs { get; set; }
    }

    /// <summary>
    /// 第三方请求打印机信息
    /// </summary>
    public class PrintSchemeInfoV2 : BaseResult
    {
        public List<ErpPrintSchemeInfoDtoV2> PrintSchemes { get; set; }
    }

    public class ErpPrintSchemeInfoDtoV2
    {
        /// <summary>
        /// 打印方案-包含打印机信息
        /// </summary>
        public ErpPrintSchemeInfoDtoV2()
        {
            DishTypeCassify = new List<SchemeDishTypeDto>();
            TableClassify = new List<SchemeTableDto>();
            VoucherList = new List<SchemeVoucherDto>();
            SetInfoList = new List<PrintSetInfoDto>();
        }

        /// <summary>
        /// 菜品分区
        /// </summary>
        public List<SchemeDishTypeDto> DishTypeCassify { get; set; }

        /// <summary>
        /// 餐桌分区
        /// </summary>
        public List<SchemeTableDto> TableClassify { get; set; }

        /// <summary>
        /// 前台单据显示(PrintCode决定)
        /// 10 点菜单、结账单
        /// 20 支付凭证、退款凭证
        /// 30 厨打总单、厨打分单
        /// </summary>
        public List<SchemeVoucherDto> VoucherList { get; set; }

        /// <summary>
        /// 前台额外配置显示(PrintCode决定)
        /// </summary>
        public List<PrintSetInfoDto> SetInfoList { get; set; }

        /// <summary>
        /// 打印机信息
        /// </summary>
        public ErpPrintConfigDtoV2 PrintConfig { get; set; }

        public int IsDefault { get; set; }
    }

    /// <summary>
    /// 打印机配置
    /// </summary>
    public class ErpPrintConfigDtoV2
    {
        public ErpPrintConfigDtoV2()
        {
            ConnStyle = 1;
            PaperType = 0;
            PaperWidth = 190;
            TopMargin = 0;
            LeftMargin = 0;
            ModifyTime = DateTime.Now;
            State = 1;
            ThirdPrintNum = 1;
        }

        public string PrintId { get; set; }

        /// <summary>
        /// 打印机Pcid
        /// </summary>
        public string Pcid { get; set; }

        /// <summary>
        /// 终端名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }

        /// <summary>
        /// 打印机别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 连接方式（1、驱动连接  2、网络连接）
        /// </summary>
        public int ConnStyle { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string ConnAddress { get; set; }

        /// <summary>
        /// 打印机品牌
        /// </summary>
        public string ConnBrand { get; set; }

        /// <summary>
        /// 连接端口
        /// </summary>
        public string ConnPort { get; set; }

        /// <summary>
        /// 纸张类型（0、未配置 1、58mm    2、76mm  3、80mm）
        /// </summary>
        public int PaperType { get; set; }

        /// <summary>
        /// 行宽度（打印纸张）,默认40mm
        /// </summary>
        public decimal PaperWidth { get; set; }

        /// <summary>
        /// 上边距，默认10mm
        /// </summary>
        public decimal TopMargin { get; set; }

        /// <summary>
        /// 左边距，默认10mm
        /// </summary>
        public decimal LeftMargin { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 是否默认打印机 1默认
        /// </summary>
        public int IsDefault { get; set; }

        /// <summary>
        /// 是否配置过 1配置过
        /// </summary>
        public int Updated { get; set; }

        /// <summary>
        /// 状态（0-伪删除（禁用）1-正常）
        /// </summary>
        public int Enable { get; set; }

        /// <summary>
        /// 打印机状态（0：异常   1：正常）
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 第三方打印的张数
        /// </summary>
        public int ThirdPrintNum { get; set; }
    }


}
