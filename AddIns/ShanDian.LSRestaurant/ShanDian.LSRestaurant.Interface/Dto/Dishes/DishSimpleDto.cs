using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class DishSimpleDto
    {
        /// <summary>
        /// 菜品Id
        /// </summary>
        public string Id { get; set; }


        /// <summary>
        /// 菜品名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 排序:按升序排
        /// </summary>
        public int Sort { get; set; }


    }
}
