using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class ChildDishDto
    {
        public string Id { get; set; }

        /// <summary>
        /// 菜品Id
        /// </summary>
        public string DishId { get; set; }

        /// <summary>
        /// 主键Id:规格Id
        /// </summary>
        public string SkusId { get; set; } = "0";

        ///// <summary>
        ///// 规格单位
        ///// </summary>
        //public string  SkusUnit { get; set; }

        /// <summary>
        /// 菜品数量（ChildDishType=10时，该字段才生效）
        /// </summary>
        public decimal Amount { get; set; } = 1;

        /// <summary>
        /// 排序：升序
        /// </summary>
        public int Sort { get; set; }
    }
}
