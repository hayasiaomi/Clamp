using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class PayDetail
    {

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
        /// 支付项名称
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
        /// 支付折扣
        /// </summary>
        public decimal PayDiscount { set; get; }

        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal UserPayAmount { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayTime { get; set; }
    }
}
