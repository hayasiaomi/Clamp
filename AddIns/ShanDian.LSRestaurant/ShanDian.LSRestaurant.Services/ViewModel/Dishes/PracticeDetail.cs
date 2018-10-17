using System;

namespace ShanDian.LSRestaurant.ViewModel.Dishes
{
    public class PracticeDetail
    {
        /// <summary>
        /// 做法组Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 做法组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 做法组排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 做法组排序时间
        /// </summary>
        public DateTime SortTime { get; set; }

        /// <summary>
        /// 做法详情Id
        /// </summary>
        public long PracticeInfoId { get; set; }

        /// <summary>
        /// 做法详情名称
        /// </summary>
        public string PracticeInfoName { get; set; }

        /// <summary>
        /// 做法详情价格
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 做法详情排序
        /// </summary>
        public int PracticeInfoSort { get; set; }

    }
}
