using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 轻餐打印菜品统计单
    /// </summary>
    public class DishStatisticsDto
    {
        /// <summary>
        /// 店名
        /// </summary>
        public string ShopName { set; get; }

        /// <summary>
        /// 子名称
        /// </summary>
        public string SubShopName { set; get; }

        /// <summary>
        /// 打印菜品统计类型：0 分类，1 菜品销量，2 退菜数量
        /// </summary>
        public string StatisticalTypeName { get; set; }

        /// <summary>
        /// 是否菜品分类
        /// </summary>
        public bool TypeFlag { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 统计的菜品(分类)数量
        /// </summary>
        public List<DishStatisticsList> Dishes { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintTime { get; set; }
    }

    public class DishStatisticsList
    {
        /// <summary>
        /// 菜品名称或分类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 总菜品数量
        /// </summary>
        public decimal TotalDishAmount { get; set; }
    }
}
