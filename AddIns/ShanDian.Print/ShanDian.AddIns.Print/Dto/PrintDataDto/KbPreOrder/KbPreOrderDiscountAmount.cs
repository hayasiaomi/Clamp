using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    /// <summary>
    /// 预点订单的支付折扣
    /// </summary>
    public class KbPreOrderDiscountAmount
    {
        /// <summary>
        /// 折扣名称
        /// </summary>
        public string DiscountName { set; get; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal Price { set; get; }
    }
}
