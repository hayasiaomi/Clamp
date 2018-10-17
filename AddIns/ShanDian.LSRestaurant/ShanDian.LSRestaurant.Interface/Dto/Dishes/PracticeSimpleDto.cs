using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class PracticeSimpleDto
    {
        /// <summary>
        /// 做法组Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 做法组是否必选
        /// </summary>
        public int IsMust { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
