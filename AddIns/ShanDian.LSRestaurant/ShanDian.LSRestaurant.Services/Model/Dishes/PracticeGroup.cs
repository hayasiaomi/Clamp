using System;
using Dapper.Contrib.Extensions;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.Factories;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    /// <summary>
    /// 菜品做法明细组
    /// </summary>
    [Table("tb_practicegroup")]
    public class PracticeGroup
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [ExplicitKey]
        public long Id { get; set; } = KeyFactory.GetPrimaryKey(DishKeyEnum.PracticeGroup);

        /// <summary>
        /// 做法组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 排序修改时间
        /// </summary>
        public DateTime SortTime { get; set; }

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
