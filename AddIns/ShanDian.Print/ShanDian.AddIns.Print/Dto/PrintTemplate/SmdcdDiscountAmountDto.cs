using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 收银汇总单的支付折扣
    /// </summary>
    public class SmdcdDiscountAmountDto
    {
        /// <summary>
        /// 折扣名称
        /// </summary>
        public string DiscountName { set; get; }

        /// <summary>
        /// 支付名称
        /// </summary>
        public string PayTypeName { set; get; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Price { set; get; }
    }
}
