using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.LSRestaurant.Interface.Dto.Dishes
{
    public class PracticeDetailDto
    {
        /// <summary>
        /// 做法组Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 做法组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 做法组是否必选:0 非，1 必选
        /// </summary>
        public int IsMust { get; set; } = 0;

        /// <summary>
        /// 做法详情List
        /// </summary>
        public List<PracticeInfoDto> PracticeInfoList { get; set; }

    }
}
