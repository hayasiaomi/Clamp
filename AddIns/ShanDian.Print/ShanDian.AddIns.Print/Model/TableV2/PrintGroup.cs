using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 方案组
    /// </summary>
    [Table("tb_printGroup")]
    public class PrintGroup
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 方案组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 方案组编号
        /// 10 点菜打印方案
        /// 20 支付打印方案
        /// 30 后厨打印方案
        /// 40 预点打印方案
        /// </summary>
        public int PrintCode{ get; set; }

        /// <summary>
        /// 方案组状态(0 禁用 1 启用)
        /// </summary>
        public int GroupState { get; set; }

        /// <summary>
        /// 方案组创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
