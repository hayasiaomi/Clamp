using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class DishSummaryDto
    {
        /// <summary>
        /// 菜品Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 菜品编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否上架状态：0 否，1是；默认1
        /// </summary>
        public int IsOnline { get; set; } = 1;

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 排序:按升序排
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜品类型： 10：普通菜品 15：加料辅菜 20：计重菜 30：加料菜（有加料辅菜的普通菜） 40：固定套餐 50：换菜套餐 60：多选套餐 70：拼合菜
        /// </summary>
        public int DishType { get; set; } = 10;

        /// <summary>
        /// 是否被其他（套餐、菜品）使用：0 不被使用，1 套餐使用，2 加料辅菜使用
        /// </summary>
        public int IsUsed { get; set; } = 0;

    }
}
