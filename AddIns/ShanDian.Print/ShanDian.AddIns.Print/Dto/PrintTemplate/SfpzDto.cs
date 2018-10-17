using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 支付凭证
    /// </summary>
    public class SfpzDto
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
        /// 应收金额
        /// </summary>
        public decimal ShouldAmount { set; get; }
        /// <summary>
        /// 支付折扣
        /// </summary>
        public decimal PayDiscount { set; get; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal ActualAmount { set; get; }
        /// <summary>
        /// 用户实付
        /// </summary>
        public decimal UserPayment { set; get; }
        /// <summary>
        /// 支付说明
        /// </summary>
        public string PaymentDescription { set; get; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PaymentDate { set; get; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PaymentTypeName { set; get; }
        /// <summary>
        /// 支付账号
        /// </summary>
        public string PaymentAccount { set; get; }
        /// <summary>
        /// 支付凭证
        /// </summary>
        public string PaymentVoucherId { set; get; }
        /// <summary>
        /// 账单流水
        /// </summary>
        public string BillRunningId { set; get; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintingDate { set; get; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// 纳税人号
        /// </summary>
        public string TaxpayerNo { get; set; }

        /// <summary>
        /// 菜品列表
        /// </summary>
        public List<SmdcdDishDto> Dishes { set; get; }

        /// <summary>
        /// 是否打印订单菜品
        /// </summary>
        public bool IsPrintDish { get; set; }

        /// <summary>
        /// 是否打印订单菜品金额
        /// </summary>
        public bool IsPrintDishAamount { get; set; }
    }
}
