using System;
using Dapper.Contrib.Extensions;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.Factories;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    /// <summary>
    /// 子菜品
    /// </summary>
    [Table("tb_childdish")]
    public class ChildDish
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [ExplicitKey]
        public long Id { get; set; } = KeyFactory.GetPrimaryKey(DishKeyEnum.ChildDish);

        /// <summary>
        /// 菜品组id
        /// </summary>
        public long DishGroupId { get; set; }

        /// <summary>
        /// 菜品Id
        /// </summary>
        public long DishId { get; set; }

        /// <summary>
        /// 主键Id:规格Id
        /// </summary>
        public long SkusId { get; set; }

        /// <summary>
        /// 菜品数量（ChildDishType=10时，该字段才生效）
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 排序：升序
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
