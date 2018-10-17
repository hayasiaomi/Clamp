using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Framework.Model
{
    public class Computer
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }

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
        public string RunMode { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpString { get; set; }

        public int MainListener { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
