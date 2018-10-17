using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 退款凭证
    /// </summary>
    public class TkpzDto
    {
        /// <summary>
        /// 店名
        /// </summary>
        public string ShopName { set; get; }

        /// <summary>
        /// 子名称
        /// </summary>
        public string SubShopName { set; get; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { set; get; }
        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime RefundDate { set; get; }

        /// <summary>
        /// 退款账号
        /// </summary>
        public string RefundAccount { set; get; }

        /// <summary>
        /// 退款方式
        /// </summary>
        public string RefundTypeName { set; get; }

        /// <summary>
        /// 支付凭证
        /// </summary>
        public string PaymentVoucherId { set; get; }

        /// <summary>
        /// 退款凭证
        /// </summary>
        public string RefundVoucherId { set; get; }

        /// <summary>
        /// 操作人员
        /// </summary>
        public string Operator { set; get; }

        /// <summary>
        /// 退款说明
        /// </summary>
        public string RefundDescription { set; get; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintingDate { set; get; }
    }
}
