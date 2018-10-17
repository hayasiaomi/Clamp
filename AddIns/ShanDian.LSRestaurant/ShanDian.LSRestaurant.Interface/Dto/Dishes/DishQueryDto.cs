using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class DishQueryDto
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; } = 16;

        /// <summary>
        /// 菜品分类Id
        /// </summary>
        public string CategoryId { get; set; } = "0";

        /// <summary>
        /// 模糊搜索关键词（菜品名称、拼音、编码）
        /// </summary>
        public string Keyword { get; set; } = String.Empty;
    }
}
