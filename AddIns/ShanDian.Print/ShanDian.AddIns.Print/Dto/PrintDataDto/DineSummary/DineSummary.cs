using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    /// <summary>
    /// 堂食汇总单
    /// </summary>
    public class DineSummary : PrintData
    {
        public DineSummary()
        {
            DiscountDetails = new List<DiscountDetail>();
            PayDetails = new List<PayDetail>();
            ActualPayTypeAmounts = new List<ActualPayTypeAmount>();
        }

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

        #region 20180823新增打赏
        /// <summary>
        /// 打赏笔数
        /// </summary>
        public int RewardsCount { set; get; }

        /// <summary>
        /// 打赏金额
        /// </summary>
        public decimal RewardsTotalAmount { set; get; }
        #endregion

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
        /// 支付折扣列表
        /// </summary>
        public List<DiscountDetail> DiscountDetails { set; get; }

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
        public List<PayDetail> PayDetails { set; get; }

        /// <summary>
        /// 退款笔数
        /// </summary>
        public decimal RefundCount { set; get; }

        /// <summary>
        /// 退款记录列表
        /// </summary>
        public List<RefundDetail> RefundDetails { set; get; }

        /// <summary>
        /// 支付项实收记录列表
        /// </summary>
        public List<ActualPayTypeAmount> ActualPayTypeAmounts { set; get; }

        /// <summary>
        /// 支付项退款记录列表
        /// </summary>
        public List<ActualPayTypeAmount> RefundPayTypeAmounts { set; get; }
    }
}
