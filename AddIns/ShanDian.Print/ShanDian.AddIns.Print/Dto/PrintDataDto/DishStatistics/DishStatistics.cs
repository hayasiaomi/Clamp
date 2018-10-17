using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    /// <summary>
    /// 轻餐打印菜品统计单
    /// </summary>
    public class DishStatistics : PrintData
    {
        DishStatistics()
        {
            StatisticalType = 0;
            Dishes = new List<DishAmount>();
        }

        /// <summary>
        /// 打印菜品统计类型：0 分类，1 菜品销量，2 退菜数量
        /// </summary>
        public int StatisticalType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 统计的菜品(分类、数量)
        /// </summary>
        public List<DishAmount> Dishes { get; set; }
    }

    public class DishAmount
    {
        /// <summary>
        /// 菜品名称或分类名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜品数量总计
        /// </summary>
        public decimal TotalDishAmount { get; set; }
    }
}
