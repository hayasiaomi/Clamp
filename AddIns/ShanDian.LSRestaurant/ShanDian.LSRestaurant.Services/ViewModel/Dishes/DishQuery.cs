using System;

namespace ShanDian.LSRestaurant.ViewModel.Dishes
{
    public class DishQuery
    {
        /// <summary>
        /// 菜品分类Id
        /// </summary>
        public long CategoryId { get; set; } = -1;

        /// <summary>
        /// 菜品类型
        /// </summary>
        public int[] DishType { get; set; } = null;

        /// <summary>
        /// 搜索关键词（模糊匹配：菜品名称、菜品编码、菜品拼音）
        /// </summary>
        public string Keyword { get; set; } = String.Empty;

        /// <summary>
        /// 菜品Id
        /// </summary>
        public long DishId { get; set; } = -1;

        /// <summary>
        /// 菜品名称
        /// </summary>
        public string DishName { get; set; } = String.Empty;
    }
}
