using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class TakeOutOrder : PrintData
    {
        public TakeOutOrder()
        {
            Dishes = new List<TkOrderDish>();
            Others = new List<TkOtherList>();
        }

        /// <summary>
        /// 单据类型
        /// （ 
        ///     100. 下单成功
        ///     101. 下单失败
        /// ）
        /// </summary>
        public int TkOrderCode { get; set; } 

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 外卖平台+订单编号
        /// </summary>
        public string BillOrderOrigin { get; set; }

        /// <summary>
        /// 配送方式
        /// </summary>
        public string SendMethord { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// 纳税人号
        /// </summary>
        public string TaxpayerNo { get; set; }

        /// <summary>
        /// 送达时间
        /// </summary>
        public string TakeMealTimer { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 菜品列表
        /// </summary>
        public List<TkOrderDish> Dishes { set; get; }

        /// <summary>
        /// 其他信息(包含了餐盒费、配送费、优惠等信息)
        /// </summary>
        public List<TkOtherList> Others { set; get; }

        /// <summary>
        /// 账单金额
        /// </summary>
        public decimal TotalAmount { set; get; }

        /// <summary>
        /// 顾客实付金额
        /// </summary>
        public decimal ShouldAmount { set; get; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// 配送地址
        /// </summary>
        public string CustomerAddress { get; set; }
    }

    public class TkOrderDish
    {
        private TkOrderDish()
        {
            SubDishes = new List<TkOrderDishSet>();
        }

        /// <summary>
        /// 菜品名 + 单位信息
        /// </summary>
        public string DishName { get; set; }

        /// <summary>
        /// 菜品数量
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 菜品单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 菜品数量是否重量单位
        /// </summary>
        public bool IsWeight { get; set; }

        /// <summary>
        /// 是否是套餐(是套餐：1，不是套餐：0)
        /// </summary>
        public bool IsSetMeal { get; set; }

        /// <summary>
        /// 记重菜单位信息(斤、两、克)
        /// </summary>
        public string WeightUnit { set; get; }

        /// <summary>
        /// 做法信息
        /// </summary>
        public string TagInfo { set; get; }

        /// <summary>
        /// 套餐子菜品
        /// </summary>
        public List<TkOrderDishSet> SubDishes { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string DishRemark { get; set; }
    }

    public class TkOrderDishSet
    {
        /// <summary>
        /// 子菜品名 + 子菜品数量 + 单位信息
        /// </summary>
        public string DishSetNameInfo { get; set; }

        /// <summary>
        /// 做法信息
        /// </summary>
        public string TagInfo { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string DishRemark { get; set; }
    }

    public class TkOtherList
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }
    }
}
