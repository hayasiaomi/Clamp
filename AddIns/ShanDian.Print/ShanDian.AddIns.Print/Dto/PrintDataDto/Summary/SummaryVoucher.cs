using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    /// <summary>
    /// 营业汇总单
    /// </summary>
    public class SummaryVoucher : PrintData
    {
        /// <summary>
        /// 账单笔数
        /// </summary>
        public string BillFlowCount { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 菜品总额
        /// </summary>
        public decimal DishCostTotalAmount { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal ReceiptsTotalAmount { get; set; }

        /// <summary>
        /// 各支付项合计集合
        /// </summary>
        public List<ReceiptsDetail> ReceiptsDetailList { get; set; }

        /// <summary>
        /// 附加总金额合计集合
        /// </summary>
        public List<OrderDiscountSimpleDto> OrderDiscountList { get; set; }
    }

    public class ReceiptsDetail
    {
        /// <summary>
        /// 支付金额（单位：元）
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 支付名称
        /// </summary>
        public string PayName { get; set; }

    }
    public class OrderDiscountSimpleDto
    {
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalDiscountMoney { get; set; }
        /// <summary>
        /// 优惠、附加费名称
        /// </summary>
        public string DiscountName { get; set; }
    }
}
