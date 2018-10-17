using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class ChildDishSummaryDto
    {
        /// <summary>
        /// 子菜品主键Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 菜品分类Id
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        /// 子菜品Id
        /// </summary>
        public string DishId { get; set; }

        /// <summary>
        /// 子菜品名称
        /// </summary>
        public string DishName { get; set; }

        /// <summary>
        /// 主键Id:规格Id
        /// </summary>
        public string SkusId { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

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
