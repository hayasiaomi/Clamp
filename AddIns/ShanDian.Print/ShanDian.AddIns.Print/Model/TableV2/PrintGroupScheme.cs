using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 门店方案
    /// </summary>
    [Table("tb_printGroupScheme")]
    public class PrintGroupScheme
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 打印机Pcid
        /// (为了区分是谁设置的打印方案)
        /// </summary>
        public string Pcid { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 打印机配置ID
        /// </summary>
        public string PrintId { get; set; }

        /// <summary>
        /// 是否默认（1 默认 0 不默认）
        /// </summary>
        public int IsDefault { get; set; }

        /// <summary>
        /// 方案状态（0:禁用 1 启用）
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 方案组对应ID
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 方案创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 方案修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }
    }
}
