using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.View
{
    public class PrintConfigPageVDto
    {
        PrintConfigPageVDto()
        {
            ConnStyle = 1;
            PaperType = 0;
        }
        public string PrintId { get; set; }
        /// <summary>
        /// 打印机Pcid
        /// </summary>
        public string Pcid { get; set; }

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
        /// 纸张类型（0、未配置 1、58mm    2、76mm  3、80mm）
        /// </summary>
        public int PaperType { get; set; }

        /// <summary>
        /// 终端名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 打印机状态（0：异常   1：正常）
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 状态（0-伪删除（禁用）1-正常）
        /// </summary>
        public int Enable { get; set; }

        /// <summary>
        /// 是否配置过 1配置过
        /// </summary>
        public int Updated { get; set; }
    }
}
