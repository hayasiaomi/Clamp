using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class RestaurantShifts : PrintData
    {
        RestaurantShifts()
        {

            Dishes = new List<DishAmount>();
        }

        /// <summary>
        /// 上班时间
        /// </summary>
        public DateTime TimeWork { get; set; }

        /// <summary>
        /// 交班时间
        /// </summary>

        public DateTime TimeShift { get; set; }

        /// <summary>
        /// 交班终端
        /// </summary>
        public string ShiftMachine { get; set; }

        /// <summary>
        /// 交班人员
        /// </summary>
        public string ShiftOperator { get; set; }

        /// <summary>
        /// 上班余额（元）
        /// </summary>
        public decimal WorkBalance { get; set; }

        /// <summary>
        /// 现金实收（元）
        /// </summary>
        public decimal ReceiptsCash { get; set; }

        /// <summary>
        /// 上缴营业额（元）
        /// </summary>
        public decimal Turnover { get; set; }

        /// <summary>
        /// 下放备用金（元）
        /// </summary>
        public decimal PettyCash { get; set; }

        /// <summary>
        /// 是否打印营业汇总
        /// </summary>
        public bool IsPrintSummary { get; set; }

        /// <summary>
        /// 打印营业汇总详情
        /// </summary>
        public SummaryShifts PrintSummaryShifts { get; set; }

        /// <summary>
        /// 是否打印菜品销量
        /// </summary>
        public bool IsPrintDish { get; set; }

        /// <summary>
        /// 统计的菜品(分类、数量)
        /// </summary>
        public List<DishAmount> Dishes { get; set; }

        /// <summary>
        /// 是否打印退菜菜品
        /// </summary>
        public bool IsPrintRefundDish { get; set; }

        /// <summary>
        /// 统计的菜品(分类、数量)
        /// </summary>
        public List<DishAmount> RefundDish { get; set; }
    }

    public class SummaryShifts
    {
        public SummaryShifts()
        {
            ReceiptsDetailList = new List<ReceiptsDetail>();
            OrderDiscountList = new List<OrderDiscountSimpleDto>();
        }

        /// <summary>
        /// 账单笔数
        /// </summary>
        public string BillFlowCount { get; set; }

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
}
