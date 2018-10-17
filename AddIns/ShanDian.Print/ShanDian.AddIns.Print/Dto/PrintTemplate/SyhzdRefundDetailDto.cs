using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 收银汇总单的退款详单
    /// </summary>
    public class SyhzdRefundDetailDto
    {
        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime RefundDate { get; set; }

        /// <summary>
        /// 支付凭证号
        /// </summary>
        public string PaymentVoucherId { get; set; }

        /// <summary>
        /// 退款凭证号
        /// </summary>
        public string RefundVoucherId { get; set; }

        /// <summary>
        /// 退款方式
        /// </summary>
        public string RefundTypeName { set; get; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }
    }
}
