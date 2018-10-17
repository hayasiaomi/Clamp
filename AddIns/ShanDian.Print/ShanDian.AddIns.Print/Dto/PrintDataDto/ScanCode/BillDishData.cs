using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class BillDishData
    {
        /// <summary>
        ///     菜品ID
        /// </summary>
        public string DishId { get; set; }

        /// <summary>
        ///     菜品名
        /// </summary>
        public string DishName { get; set; }

        /// <summary>
        ///     订单菜品ID（套餐菜品为空）
        /// </summary>
        public string BillDishId { get; set; }

        /// <summary>
        ///     菜品数量
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 菜品数量是否重量单位
        /// </summary>
        public bool IsWeight { get; set; }

        /// <summary>
        ///     菜品单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     是否是套餐(是套餐：1，不是套餐：0)
        /// </summary>
        public bool IsSetMeal { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
