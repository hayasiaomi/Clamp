using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 扫码点菜单
    /// </summary>
    public class SmdcdDto
    {

        public SmdcdDto()
        {
            this.Dishes = new List<SmdcdDishDto>();
            this.PayTypeAmounts = new List<SmdcdPayTypeAmountDto>();
            this.ExceptionDishes = new List<SmdcdDishDto>();
            this.DiscountAmounts = new List<SmdcdDiscountAmountDto>();
            this.PriceDetails = new List<SmdcdPriceDetail>();
            FooterAlignment = "left";
        }

        /// <summary>
        /// 是否结账
        /// </summary>
        public bool IsCheckout { set; get; }

        /// <summary>
        /// 是否下单失败
        /// </summary>
        public bool IsFailured { set; get; }

        /// <summary>
        /// 是否轻餐版扫码结账单
        /// </summary>
        public bool IsMealCheckout { set; get; }

        /// <summary>
        /// 店名
        /// </summary>
        public string ShopName { set; get; }

        /// <summary>
        /// 子名称
        /// </summary>
        public string SubShopName { set; get; }

        /// <summary>
        /// 取餐模式下单餐桌类型(1. 桌牌号 2. 取餐号)
        /// </summary>
        public string TableType { set; get; }

        /// <summary>
        /// 桌牌号
        /// </summary>
        public string TableNo { set; get; }

        /// <summary>
        /// 单号
        /// </summary>
        public string BillNo { set; get; }

        /// <summary>
        /// 人数
        /// </summary>
        public int PersonCount { set; get; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderTakeDate { set; get; }

        /// <summary>
        /// 整单备注
        /// </summary>
        public string Remark { set; get; }

        /// <summary>
        /// 菜品列表
        /// </summary>
        public List<SmdcdDishDto> Dishes { set; get; }

        /// <summary>
        /// 合计
        /// </summary>
        public decimal TotalAmount { set; get; }

        /// <summary>
        /// 支付折扣列表
        /// </summary>
        public List<SmdcdDiscountAmountDto> DiscountAmounts { set; get; }


        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal ShouldAmount { set; get; }

        /// <summary>
        /// 实收金额的支付方式列表
        /// </summary>
        public List<SmdcdPayTypeAmountDto> PayTypeAmounts { set; get; }

        /// <summary>
        ///   发票抬头
        /// </summary>
        public string InvoiceTitle { set; get; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxpayerIdentify { set; get; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime PrintingDate { set; get; }

        /// <summary>
        /// 异常菜品
        /// </summary>
        public List<SmdcdDishDto> ExceptionDishes { set; get; }

        /// <summary>
        ///  异常原因
        /// </summary>
        public string ExceptionReason { set; get; }

        /// <summary>
        /// 异常菜品的数量
        /// </summary>
        public int TotalDishCount { set; get; }

        /// <summary>
        /// 异常菜品的合计金额
        /// </summary>
        public decimal TotalDishPrice { set; get; }

        /// <summary>
        /// 页脚
        /// </summary>
        public string Footer { set; get; }

        /// <summary>
        /// 页脚对齐
        /// </summary>
        public string FooterAlignment { set; get; }

        /// <summary>
        /// 附加费用信息
        /// </summary>
        public List<SmdcdPriceDetail> PriceDetails { get; set; }

        /// <summary>
        /// 尾数调整（单位：元）
        /// </summary>
        public string ErasingMoney { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public string OrderOrigin { get; set; }
    }
    public class SmdcdPriceDetail
    {
        /// <summary>
        /// 附加费金额（单位：元）
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 附加费名称
        /// </summary>
        public string PriceTypeName { get; set; }
    }
}
