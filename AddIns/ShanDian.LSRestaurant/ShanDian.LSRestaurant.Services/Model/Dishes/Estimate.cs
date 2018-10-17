using System;
using Dapper.Contrib.Extensions;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.Factories;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    /// <summary>
    /// 菜品估清
    /// </summary>
    [Table("tb_estimate")]
    public class Estimate
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        [ExplicitKey]
        public long Id { get; set; } = KeyFactory.GetPrimaryKey(DishKeyEnum.Estimate);

        /// <summary>
        /// 菜品Id
        /// </summary>
        public long DishId { get; set; }

        /// <summary>
        /// 估清数量：0时为估清状态，默认0
        /// </summary>
        public decimal Amount { get; set; }

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
