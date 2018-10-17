using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.Restaurant
{
    public class RestaurantDto
    {
        public string Id { get; set; }

        /// <summary>
        /// 餐厅名称
        /// </summary>
        public string Name { get; set; }

        public bool IsDisabled { get; set; }

        /// <summary>
        /// 品牌ID
        /// </summary>
        public string BrandId { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// 餐厅子名
        /// </summary>
        public string SubName { get; set; }

        /// <summary>
        /// 餐厅的图片LOGO
        /// </summary>
        public string Logo { get; set; }

        public string Phone { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 平台餐厅ID
        /// </summary>
        public string MikeRestaurantId { get; set; }

        public string BusinessHours { get; set; }
    }
}
