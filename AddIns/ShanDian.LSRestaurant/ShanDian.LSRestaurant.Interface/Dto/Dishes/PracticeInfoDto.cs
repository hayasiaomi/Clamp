using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class PracticeInfoDto
    {
        /// <summary>
        /// 做法详情Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 做法详情名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 做法详情价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

    }
}
