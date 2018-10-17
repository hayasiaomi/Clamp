using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class DishMenuPageDto
    {
        /// <summary>
        /// 菜品Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>

        public string Name { get; set; }

        /// <summary>
        /// 价格
        /// </summary>

        public decimal Price { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜品估清数量： -1 不估清，0 已估清，>0 估清剩余数量
        /// </summary>
        public decimal EstimateCnt { get; set; } = -1;
    }
}
