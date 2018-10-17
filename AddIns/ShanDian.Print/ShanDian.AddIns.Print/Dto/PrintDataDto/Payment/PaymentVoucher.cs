using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class PaymentVoucher : PrintData
    {
        public PaymentVoucher()
        {
            BillDish=new List<OrderDishDto>();
            IsPrintDish = false;
            IsPrintDishAamount = false;
        }
        /// <summary>
        /// 支付凭证号
        /// </summary>
        public string TradeNo { get; set; }

        /// <summary>
        /// 支付类型
        /// 10 支付宝
        /// 20 微信
        /// 30 银联
        /// 31 银联商务
        /// 32 银联在线
        /// 33 湖南银联天天掌柜
        /// 50 储值卡
        /// 90 其他方式
        /// </summary>
        public int OnLinePayType { get; set; }

        /// <summary>
        /// 支付方式名称
        /// </summary>
        public string PayName { get; set; }

        /// <summary>
        /// 应收金额
        /// </summary>
        public decimal Totalamount { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal Receiptamount { get; set; }

        /// <summary>
        /// 用户实付
        /// </summary>
        public decimal UserPayAmount { get; set; }

        /// <summary>
        /// 支付说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayTime { get; set; }

        /// <summary>
        /// 支付账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 支付折扣
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 账单流水
        /// </summary>
        public string BillRunningId { set; get; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// 纳税人号
        /// </summary>
        public string TaxpayerNo { get; set; }

        /// <summary>
        /// 订单内菜品
        /// </summary>
        public List<OrderDishDto> BillDish { get; set; }

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
