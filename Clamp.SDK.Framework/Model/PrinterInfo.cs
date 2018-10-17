using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Model
{
    public class PrinterInfo
    {
        public int Id { get; set; }

        public string PCID { set; get; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }

        /// <summary>
        /// 打印机状态（0：异常   1：正常）
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 状态（0-伪删除（禁用）1-正常）
        /// </summary>
        public int Enable { get; set; }
    }
}
