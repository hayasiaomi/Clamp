using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class MenuChildDishDto
    {
        public string Id { get; set; }

        /// <summary>
        /// 菜品Id
        /// </summary>
        public string DishId { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否上架状态：0 否，1是；默认1
        /// </summary>
        public int IsOnline { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }


        /// <summary>
        /// 菜品数量（ChildDishType=10时，该字段才生效）
        /// </summary>
        public decimal Amount { get; set; } = 1;

        /// <summary>
        /// 菜品类型： 10：普通菜品 15：加料辅菜 20：计重菜 30：加料菜（有加料辅菜的普通菜） 40：固定套餐 50：换菜套餐 60：多选套餐 70：拼合菜
        /// </summary>
        public int DishType { get; set; } = 10;

        /// <summary>
        /// 排序：升序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用做法：0 关，1 启用，2 启用并有必选；默认0
        /// </summary>
        public int IsPractice { get; set; } = 0;

        /// <summary>
        /// 是否启用加料菜：0 关，1 启用；默认0
        /// </summary>
        public int IsCharging { get; set; } = 0;

        /// <summary>
        /// 菜品估清数量： -1 不估清，0 已估清，>0 估清剩余数量
        /// </summary>
        public decimal EstimateCnt { get; set; } = -1;
    }
}
