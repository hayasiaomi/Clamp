using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Model
{
    public class PrinterInfo
    {

        public string PCID { set; get; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }

        /// <summary>
        /// 打印机状态（0：异常   1：正常）
        /// </summary>
        public int State { get; set; }

    }
}
