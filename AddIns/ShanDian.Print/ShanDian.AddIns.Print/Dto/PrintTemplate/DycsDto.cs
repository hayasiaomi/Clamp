using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 打印机测试页
    /// </summary>
    public class DycsDto
    {
        /// <summary>
        /// 终端名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintingName { get; set; }

        /// <summary>
        /// 打印纸类型
        /// </summary>
        public string PagerTypeName { set; get; }

        /// <summary>
        /// 行宽度（打印纸张）,默认40mm
        /// </summary>
        public int PaperWidth { get; set; }

        /// <summary>
        /// 上边距，默认10mm
        /// </summary>
        public int TopMargin { get; set; }

        /// <summary>
        /// 左边距，默认10mm
        /// </summary>
        public int LeftMargin { get; set; }

        /// <summary>
        /// 打印机别名
        /// </summary>
        public string Alias { get; set; }
    }
}
