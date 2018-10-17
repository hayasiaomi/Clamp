using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 堂食汇总单
    /// </summary>
    public class TshzdDto
    {
        public TshzdDto()
        {
            this.ActualPayTypeAmounts = new List<SyhzdPayTypeAmountDto>();
            this.RefundPayTypeAmounts = new List<SyhzdPayTypeAmountDto>();
            this.PayDetails = new List<SyhzdPayDetailDto>();
            this.DiscountAmounts = new List<SyhzDiscountAmountDto>();
        }

        /// <summary>
        /// 店名
        /// </summary>
        public string ShopName { set; get; }

        /// <summary>
        /// 子名称
        /// </summary>
        public string SubShopName { set; get; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartDate { set; get; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { set; get; }

        /// <summary>
        /// 支付笔数
        /// </summary>
        public int PayCount { set; get; }

        /// <summary>
        /// 支付总金额
        /// </summary>
        public decimal PayTotalAmount { set; get; }

        /// <summary>
        /// 打赏笔数
        /// </summary>
        public string RewardsCount { set; get; }

        /// <summary>
        /// 打赏金额
        /// </summary>
        public decimal RewardsTotalAmount { set; get; }

        /// <summary>
        /// 异常笔数
        /// </summary>
        public int AbnormalCount { set; get; }

        /// <summary>
        /// 异常总金额
        /// </summary>
        public decimal AbnormalTotalAmount { set; get; }

        /// <summary>
        /// 尾数调整笔数
        /// </summary>
        public int AdjustmentCount { set; get; }

        /// <summary>
        /// 尾数调整总金额
        /// </summary>
        public decimal AdjustmentTotalAmount { set; get; }

        /// <summary>
        /// 优惠笔数
        /// </summary>
        public int DiscountCount { set; get; }

        /// <summary>
        /// 支付折扣总金额
        /// </summary>
        public decimal PayDiscountAmount { set; get; }

        /// <summary>
        /// 支付折扣的列表
        /// </summary>
        public List<SyhzDiscountAmountDto> DiscountAmounts { set; get; }

        /// <summary>
        /// 退款总金额
        /// </summary>
        public decimal RefundTotalAmount { set; get; }

        /// <summary>
        /// 实收总金额
        /// </summary>
        public decimal ActualTotalAmount { set; get; }

        /// <summary>
        /// 是否带支付详单
        /// </summary>
        public bool IsPayDetail { set; get; }

        /// <summary>
        /// 支付详单列表
        /// </summary>
        public List<SyhzdPayDetailDto> PayDetails { set; get; }

        /// <summary>
        /// 退款笔数
        /// </summary>
        public decimal RefundCount { set; get; }

        /// <summary>
        /// 退款总金额
        /// </summary>
        public List<SyhzdPayTypeAmountDto> RefundPayTypeAmounts { set; get; }

        /// <summary>
        /// 实收总金额
        /// </summary>
        public List<SyhzdPayTypeAmountDto> ActualPayTypeAmounts { set; get; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintingDate { set; get; }
    }
}
