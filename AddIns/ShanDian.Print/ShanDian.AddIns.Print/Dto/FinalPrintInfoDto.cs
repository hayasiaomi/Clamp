using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    /// <summary>
    /// 打印信息类
    /// </summary>
    public class FinalPrintInfoDto
    {
        /// <summary>
        /// 要打印的PCID
        /// </summary>
        public string Pcid { set; get; }

        /// <summary>
        /// 顶部间距
        /// </summary>
        public int Top { set; get; }
        /// <summary>
        /// 左边间距
        /// </summary>
        public int Left { set; get; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { set; get; }
        /// <summary>
        /// 打印内容
        /// </summary>
        public string PrintingBody { set; get; }
        /// <summary>
        /// 打印机名
        /// </summary>
        public string PrintingName { set; get; }

        /// <summary>
        /// 打印次数
        /// </summary>
        public int Copies { set; get; }
    }
}
