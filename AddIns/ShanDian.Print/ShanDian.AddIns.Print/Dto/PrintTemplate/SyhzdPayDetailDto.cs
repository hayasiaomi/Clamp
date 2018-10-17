using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 收银汇总单的支付详单
    /// </summary>
    public class SyhzdPayDetailDto
    {
        public SyhzdPayDetailDto()
        {
            this.RefundDetails = new List<SyhzdRefundDetailDto>();
        }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PaymentDate { set; get; }
        /// <summary>
        /// 支付凭证号
        /// </summary>
        public string PaymentVoucherId { set; get; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PaymentAmount { set; get; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal ActualAmount { set; get; }

        /// <summary>
        /// 支付折扣
        /// </summary>
        public decimal PayDiscount { set; get; }


        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentTypeName { set; get; }

        /// <summary>
        /// 退款记录列表
        /// </summary>
        public List<SyhzdRefundDetailDto> RefundDetails { set; get; }
    }
}
