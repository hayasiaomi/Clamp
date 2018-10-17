using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 全局信息（目前打印专用）
    /// </summary>
    [Table("tb_printSetInfo")]
    public class PrintSetInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 名称描述信息
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 属性(string是最稳定的)
        /// 100 对接配置
        /// 101 下单失败单
        /// 102 自动核销失败单
        /// 103 套餐按子菜品分单打印
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 属性范围(打印方案、门店方案)
        /// 0：标识当前系统是否对接
        /// 1：方案组
        /// 2：门店方案
        /// </summary>
        public int Range { get; set; }

        /// <summary>
        /// 设置关联ID
        /// </summary>
        public int CombineId { get; set; }
    }
}
