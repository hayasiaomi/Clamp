using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Machine.ViewModel
{
    class VMComputer
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 机器码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 机器名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 机器类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
