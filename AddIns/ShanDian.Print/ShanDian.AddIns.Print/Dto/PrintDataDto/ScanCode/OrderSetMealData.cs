using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class OrderSetMealData
    {
        /// <summary>
        /// 菜品ID
        /// </summary>
        public string DishId { get; set; }
        /// <summary>
        /// 菜品名
        /// </summary>
        public string DishName { get; set; }
        /// <summary>
        /// 订单菜品ID（套餐菜品为空）
        /// </summary>
        public string BillDishId { get; set; }
        /// <summary>
        /// 菜品数量
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 菜品单价
        /// </summary>
        public decimal Price { get; set; }

        public string SetMealDishId { get; set; }
    }
}
