using System;
using Dapper.Contrib.Extensions;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.Factories;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    /// <summary>
    /// 菜品组
    /// </summary>
    [Table("tb_dishgroup")]
    public class DishGroup
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [ExplicitKey]
        public long Id { get; set; } = KeyFactory.GetPrimaryKey(DishKeyEnum.DishGroup);


        /// <summary>
        /// 菜品组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜品Id
        /// </summary>
        public long DishId { get; set; }

        /// <summary>
        /// 菜品分类Id
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 菜品组类型： 10：套餐子菜品 20：可更换菜列表 30：多选套餐组  40：加料菜辅菜 50：拼合主菜 60：拼合辅菜
        /// </summary>
        public int GroupType { get; set; } = 40;

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 点菜规则,预留，目前普通菜、固定套餐用不到(key、value格式)
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
