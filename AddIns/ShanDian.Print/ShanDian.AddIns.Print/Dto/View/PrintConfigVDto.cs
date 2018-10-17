using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.View
{
    public class PrintConfigVDto
    {
        public PrintConfigVDto()
        {
            ConnStyle = 1;
            PaperType = 0;
            PaperWidth = 40;
            TopMargin = 10;
            LeftMargin = 10;
            ModifyTime = DateTime.Now;
        }
        public int LocalPrintId { get; set; }

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

        public List<PrintSchemeDto> PrintSchemeList { get; set; }
    }
}
