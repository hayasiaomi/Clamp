namespace ShanDian.LSRestaurant.ViewModel.Dishes
{
    public class DishPracticeDetail
    {
        /// <summary>
        /// 菜品分类Id
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 做法组Id
        /// </summary>
        public long PracticeGroupId { get; set; }

        /// <summary>
        /// 做法规则
        /// </summary>
        public string DishRule { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 做法组名称
        /// </summary>
        public string PracticeGroupName { get; set; }

        /// <summary>
        /// 做法详情Id
        /// </summary>
        public long PracticeInfoId { get; set; }

        /// <summary>
        /// 做法详情名称
        /// </summary>
        public string PracticeInfoName { get; set; }

        /// <summary>
        /// 做法详情加价
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 做法详情排序
        /// </summary>
        public int PracticeInfoSort { get; set; }
    }
}
