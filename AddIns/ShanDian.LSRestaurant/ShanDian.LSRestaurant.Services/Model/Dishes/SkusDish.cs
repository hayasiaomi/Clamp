using System;
using Dapper.Contrib.Extensions;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.Factories;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    /// <summary>
    /// 菜品与份量关系表
    /// </summary>
    [Table("tb_skusdish")]
    public class SkusDish
    {
        /// <summary>
        /// 主键Id:份量Id
        /// </summary>
        [ExplicitKey]
        public long SkusId { get; set; } = KeyFactory.GetPrimaryKey(DishKeyEnum.SkusDish);

        /// <summary>
        /// 菜品id
        /// </summary>
        public long DishId { get; set; }

        /// <summary>
        /// 份量名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 价格,单位【分】
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModificationTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifitor { get; set; }
    }
}
