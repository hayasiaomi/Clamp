using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    public class PrintSummaryDto
    {
        /// <summary>
        /// 店名
        /// </summary>
        public string ShopName { set; get; }

        /// <summary>
        /// 分店名
        /// </summary>
        public string SubShopName { set; get; }

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
        public List<ShanDian.AddIns.Print.Dto.PrintDataDto.ReceiptsDetail> ReceiptsDetailList { get; set; }

        /// <summary>
        /// 附加总金额合计集合
        /// </summary>
        public List<ShanDian.AddIns.Print.Dto.PrintDataDto.OrderDiscountSimpleDto> OrderDiscountList { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintTime { get; set; }
    }
}
