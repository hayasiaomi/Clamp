using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class CategoryDishSummaryDto
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 分类名称(唯一)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否隐藏：0 显示，1隐藏
        /// </summary>
        public int IsHidden { get; set; }

        /// <summary>
        /// 是否有做法：0 无，1 有；
        /// </summary>
        public int IsPractice { get; set; } = 0;


        /// <summary>
        /// 是否启用加料菜：0 关，1 公共加料，2 私有加料；默认0
        /// </summary>
        public int IsCharging { get; set; } = 0;

        /// <summary>
        /// 分类下的菜品数量
        /// </summary>
        public int DishCnt { get; set; } = 0;
    }
}
