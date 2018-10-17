using System;
using Dapper.Contrib.Extensions;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.Factories;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    /// <summary>
    /// 菜品做法关系表
    /// </summary>
    [Table("tb_practicedish")]
    public class PracticeDish
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [ExplicitKey]
        public long Id { get; set; } = KeyFactory.GetPrimaryKey(DishKeyEnum.PracticeDish);

        /// <summary>
        /// dishId
        /// </summary>
        public long DishId { get; set; }

        /// <summary>
        /// 菜品分类id
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 做法组id
        /// </summary>
        public long PracticeGroupId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 点菜规则（key、value）
        /// </summary>
        public string DishRule { get; set; }

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
