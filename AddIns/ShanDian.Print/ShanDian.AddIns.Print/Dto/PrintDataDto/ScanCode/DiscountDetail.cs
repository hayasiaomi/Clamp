using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class DiscountDetail
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 优惠名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 优惠金额
        /// </summary>
        public decimal Amount { get; set; }
    }
}
