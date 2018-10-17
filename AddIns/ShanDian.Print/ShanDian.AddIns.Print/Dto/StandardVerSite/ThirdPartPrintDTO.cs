using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;

namespace ShanDian.AddIns.Print.Dto.StandardVerSite
{
    public class ThirdPartPrint.Dto
    {
        /// <summary>
        /// 打印信息
        /// </summary>
        public string PrintInfos { set; get; }//List<PrintInfo>
    }

    public class PrintInfo
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string TemplateName { set; get; }

        /// <summary>
        /// 模板对应的数据（Json）
        /// </summary>
        public string TemplateData { set; get; }

        ///// <summary>
        ///// 打印方案集合
        ///// </summary>
        //public List<ErpPrintSchemeInfoDto> PrintSchemes { get; set; }

        /// <summary>
        /// 打印方案集合
        /// </summary>
        public List<ErpPrintConfigDto> PrintConfigs { get; set; }

    }

    public class ErpPrintSchemeInfoDto
    {
        /// <summary>
        /// 打印方案-包含打印机信息
        /// </summary>
        public ErpPrintSchemeInfoDto()
        {
            TagList = new List<ErpPrintSchemeLabelDto>();
        }

        /// <summary>
        /// 选择的打印标签
        /// </summary>
        public List<ErpPrintSchemeLabelDto> TagList { get; set; }

        /// <summary>
        /// 打印机信息
        /// </summary>
        public ErpPrintConfigDto PrintConfig { get; set; }
    }

    /// <summary>
    /// 打印机配置
    /// </summary>
    public class ErpPrintConfigDto
    {
        public ErpPrintConfigDto()
        {
            ConnStyle = 1;
            PaperType = 0;
            PaperWidth = 190;
            TopMargin = 0;
            LeftMargin = 0;
            ModifyTime = DateTime.Now;
            PrintNum = 0;
            State = 1;
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
        /// 连接方式（1、驱动连接  2、网络连接）
        /// </summary>
        public int ConnStyle { get; set; }

        /// <summary>
        /// 打印机别名
        /// </summary>
        public string Alias { get; set; }

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
        /// 打印数量
        /// </summary>
        public int PrintNum { get; set; }

        /// <summary>
        /// 打印机状态（0：异常   1：正常）
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 打印模版ID
        /// </summary>
        public int VoucherId { get; set; }

        /// <summary>
        ///  0本地打印 1为分组默认打印机 2分组打印
        /// </summary>
        public int LocalMachine { get; set; }

        /// <summary>
        /// 本地机器ID
        /// </summary>
        public string MachineId { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string SchemeCode { get; set; }

    }

    /// <summary>
    /// 打印机标签
    /// </summary>
    public class ErpPrintSchemeLabelDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 打印方案Id
        /// </summary>
        public int PrintSchemeId { get; set; }

        /// <summary>
        /// 标签类型的代号1 餐桌 2菜品
        /// </summary>
        public int LabelGroupCode { get; set; }

        /// <summary>
        /// 线上餐桌ID guid
        /// </summary>
        public string LabelId { get; set; }

        /// <summary>
        /// 线下餐桌ID guid
        /// </summary>
        public string ErpLabelId { get; set; }

        /// <summary>
        /// 餐桌名
        /// </summary>
        public string LabelName { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string SchemeCode { get; set; }
    }
}
