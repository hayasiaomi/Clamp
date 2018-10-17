using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class RefundDetail
    {
        /// <summary>
        /// 支付凭证号
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 退款凭证号
        /// </summary>
        public string RefundTradeNo { get; set; }

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
        /// 退款金额
        /// </summary>
        public decimal RefundAmount { get; set; }

        /// <summary>
        /// 退款方式
        /// </summary>
        public string RefundTypeName { set; get; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime RefundTime { get; set; }

        /// <summary>
        /// 支付账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string Operator { get; set; }
    }
}
