using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class KbPreOrder : PrintData
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
        /// 下单时间
        /// </summary>
        public DateTime OrderTime { get; set; }

        /// <summary>
        /// 取餐号
        /// </summary>
        public string TakeMealNum { get; set; }

        /// <summary>
        /// 就餐方式
        /// </summary>
        public string Scene { get; set; }

        /// <summary>
        /// 联系人
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// 顾客就餐时间
        /// </summary>
        public DateTime EatingTime { get; set; }

        /// <summary>
        /// 就餐人数
        /// </summary>
        public int CustomerCount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 菜品列表
        /// </summary>
        public List<KbPreOrderDish> Dishes { set; get; }

        /// <summary>
        /// 支付折扣列表
        /// </summary>
        public List<KbPreOrderDiscountAmount> DiscountAmounts { set; get; }
        /// <summary>
        /// 附加费用列表
        /// </summary>
        public List<KbPreOrderAdditionalCost> AdditionalCharges { set; get; }

        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalAmount { set; get; }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal ShouldAmount { set; get; }
    }
}
