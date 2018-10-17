using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 打印配置表
    /// </summary>
    [Table("tb_printinfo")]
    public class PrintInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int PrintId { get; set; }

        /// <summary>
        /// 打印机Pcid
        /// </summary>
        public string Pcid { get; set; }
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
