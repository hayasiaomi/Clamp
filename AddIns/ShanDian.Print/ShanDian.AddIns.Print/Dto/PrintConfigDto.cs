using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;

namespace ShanDian.AddIns.Print.Dto
{
    /// <summary>
    /// 打印机配置
    /// </summary>
    public class PrintConfigDto
    {

        public PrintConfigDto()
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
        /// 门店Pcid
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
        /// 打印地址
        /// </summary>
        public string ConnAddress { get; set; }

        /// <summary>
        /// 打印机品牌
        /// </summary>
        public string ConnBrand { get; set; }

        /// <summary>
        /// 打印端口
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
        /// 打印数量
        /// </summary>
        public int PrintNum { get; set; }
    }
}
