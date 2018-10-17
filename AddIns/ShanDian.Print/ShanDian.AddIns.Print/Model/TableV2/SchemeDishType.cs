using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 打印方案的区域
    /// </summary>
    [Table("tb_schemeDishType")]
    public class SchemeDishType
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 打印方案Id
        /// </summary>
        public int SchemeId { get; set; }

        /// <summary>
        /// 线上分类ID
        /// </summary>
        public string MKDishTypeID { get; set; }

        /// <summary>
        /// 线下分类ID
        /// </summary>
        public string ErpDishTypeID { get; set; }

        /// <summary>
        /// 菜品分类名称
        /// </summary>
        public string DishTypeName { get; set; }
    }
}
