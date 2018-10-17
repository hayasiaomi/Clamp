using System;
using Dapper.Contrib.Extensions;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.Factories;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    /// <summary>
    /// 菜品分类
    /// </summary>
    [Table("tb_categorydish")]
    public class CategoryDish
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [ExplicitKey]
        public long Id { get; set; } = KeyFactory.GetPrimaryKey(DishKeyEnum.Category);

        /// <summary>
        /// 分类名称(唯一)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 排序时间
        /// </summary>
        public DateTime SortTime { get; set; }

        /// <summary>
        /// 是否隐藏：0 显示，1隐藏
        /// </summary>
        public int IsHidden { get; set; } = 0;

        /// <summary>
        /// 是否启用做法：0 关，1 公共做法，2私有做法；默认0
        /// </summary>
        public int IsPractice { get; set; } = 0;


        /// <summary>
        /// 是否启用加料菜：0 关，1 公共加料，2 私有加料；默认0
        /// </summary>
        public int IsCharging { get; set; } = 0;

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
