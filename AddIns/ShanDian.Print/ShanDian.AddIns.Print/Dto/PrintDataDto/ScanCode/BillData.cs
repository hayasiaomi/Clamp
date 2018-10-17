using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class BillData : PrintData
    {
        public BillData()
        {
            BillDish = new List<BillDishData>();
            OrderSetMealDatas = new List<OrderSetMealData>();
            Tags = new List<Tag>();
            FailBillDish = new List<BillDishData>();
            PayDetails = new List<PayDetail>();
            DiscountDetails = new List<DiscountDetail>();
            PriceDetails = new List<PriceDetail>();
        }

        /// <summary>
        /// 人数
        /// </summary>
        public int Person { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 餐桌名称
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal RealMoney { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单内菜品
        /// </summary>
        public List<BillDishData> BillDish { get; set; }

        /// <summary>
        /// 套餐内菜品
        /// </summary>
        public List<OrderSetMealData> OrderSetMealDatas { get; set; }

        /// <summary>
        /// 菜品标签
        /// </summary>
        public List<Tag> Tags { get; set; }

        /// <summary>
        /// 失败菜品
        /// </summary>
        public List<BillDishData> FailBillDish { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// 纳税人号
        /// </summary>
        public string TaxpayerNo { get; set; }

        /// <summary>
        /// 支付明细
        /// </summary>
        public List<PayDetail> PayDetails { get; set; }

        /// <summary>
        /// 优惠明细
        /// </summary>
        public List<DiscountDetail> DiscountDetails { get; set; }

        /// <summary>
        /// 是否整单下单失败
        /// </summary>
        public bool IsFailured { set; get; }

        /// <summary>
        /// 是否结账
        /// </summary>
        public bool IsCheckout { set; get; }

        /// <summary>
        /// 失败原因
        /// </summary>
        public string FailReason { get; set; }

        /// <summary>
        /// 附加费用信息
        /// </summary>
        public List<PriceDetail> PriceDetails { get; set; }

        /// <summary>
        /// 单据类型
        /// （  100. 扫码点菜单（默认）
        ///     101. 整单下单失败单
        ///     102. 扫码点菜结账单
        ///     103. 轻餐版专用结账单
        /// ）
        /// IsFailured、IsCheckout在这个字段有相对应的值的时候不生效
        /// </summary>
        public int CheckBillCode { get; set; } = 100;

        /// <summary>
        /// 打印扫码点菜单餐桌模式
        /// 1. 桌牌号 2. 取餐号
        /// </summary>
        public int TableTypeCode { get; set; } = 1;

        /// <summary>
        /// 尾数调整（单位：元）
        /// </summary>
        public decimal ErasingMoney { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public string OrderOrigin { get; set; }
    }
    public class PriceDetail
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
