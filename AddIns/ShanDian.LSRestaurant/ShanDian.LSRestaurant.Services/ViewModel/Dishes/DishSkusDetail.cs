namespace ShanDian.LSRestaurant.ViewModel.Dishes
{
    public class DishSkusDetail
    {
        public long Id { get; set; }

        /// <summary>
        /// 菜品名称(唯一，【菜品与加料菜之间可重名】)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜品单价，单位[分]
        /// </summary>
        public int Price { get; set; } = 0;

        /// <summary>
        /// 菜品类型： 10：普通菜品 15：加料辅菜 20：计重菜 30：加料菜（有加料辅菜的普通菜） 40：固定套餐 50：换菜套餐 60：多选套餐 70：拼合菜
        /// </summary>
        public int DishType { get; set; } = 10;

        /// <summary>
        /// 菜品分类Id
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 菜品单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 排序:按升序排
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否启用份量：0 否，1 是
        /// </summary>
        public int IsSkus { get; set; } = 0;

        /// <summary>
        /// 份量Id
        /// </summary>

        public long SkusId { get; set; }

        /// <summary>
        /// 份量名称
        /// </summary>
        public string SkusName { get; set; }

        /// <summary>
        /// 份量价格
        /// </summary>
        public int SkusPrice { get; set; }

        /// <summary>
        /// 份量排序
        /// </summary>
        public int SkusSort { get; set; }
    }
}
