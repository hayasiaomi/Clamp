using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.Restaurant
{
    public class RestaurantsInfo
    {
        /// <summary>
        /// 店名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 分店名
        /// </summary>
        public string SubName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 餐厅地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 货币符号
        /// </summary>
        public string CurrencySign { get; set; }

        /// <summary>
        /// 业务配置
        /// </summary>
        public OrderConfig OrderConfig { get; set; }
        /// <summary>
        /// 抹零配置
        /// </summary>
        public RoundoffConfig RoundOffConfig { get; set; }

        /// <summary>
        /// 是否轻餐
        /// </summary>
        public bool IsOpenQC { get; set; }
    }

    public class RoundoffConfig
    {
        /// <summary>
        /// 抹零方式
        /// </summary>
        public int RoundoffMethod { get; set; }
        /// <summary>
        /// 抹零精度
        /// </summary>
        public int Precision { get; set; }
        /// <summary>
        /// 开启抹零配置
        /// </summary>
        public bool IsRoundoff { get; set; }
    }

    public class OrderConfig
    {
        /// <summary>
        /// 模式：
        ///1先买单
        ///2后买单
        ///3先买单拼桌送餐
        ///4先买单无桌自取
        ///5自助餐模式
        /// </summary>
        public int ScanMode { get; set; }
        /// <summary>
        /// 是否先买单
        /// </summary>
        public bool IsPrePay { get; set; }
        /// <summary>
        /// 桌面码付版本
        /// 1：码付1.0
        /// 2：码付2.0
        /// 3：码付3.0
        /// </summary>
        public int CodePayMode { get; set; }
        /// <summary>
        /// 订单审核
        /// 0：未开启审核
        /// 1：超级店长审核
        /// 2：服务员下单密码审核
        /// </summary>
        public int OrderCheckType { get; set; }
        /// <summary>
        /// 用餐人数开关
        /// </summary>
        public bool DinerNum { get; set; }
        /// <summary>
        /// 餐桌收银无单支付开光
        /// </summary>
        public bool TableNotBillPay { get; set; }
        /// <summary>
        /// 下单验证手机（仅限先买单）
        /// </summary>
        public bool OrderPhoneCheck { get; set; }
        /// <summary>
        /// 扫码浏览器限制
        /// 0：不限制
        /// 1：仅限支付宝
        /// 2：仅限微信
        /// </summary>
        public int BrowserLimit { get; set; }
        /// <summary>
        /// 门店支付二维码备注
        /// </summary>
        public bool PayCodeRemark { get; set; }
        /// <summary>
        /// 呼叫取餐功能
        /// </summary>
        public bool CallTakeMeal { get; set; }
        /// <summary>
        /// 订单备注功能
        /// </summary>
        public bool OrderRemark { get; set; }
        /// <summary>
        /// 菜品备注功能
        /// </summary>
        public bool DishRemark { get; set; }
        /// <summary>
        /// 服务费
        /// </summary>
        public bool ServiceFee { get; set; }
        /// <summary>
        /// 账单金额审核（线下审核）
        /// </summary>
        public bool BillMoneyCheck { get; set; }
        /// <summary>
        /// 订单金额确认（超级店长确认）
        /// </summary>
        public bool BillMoneyConfirm { get; set; }
        /// <summary>
        /// 支付前屏蔽订单总结（扫码点菜功能）
        /// </summary>
        public bool HideMoneyBeforePay { get; set; }
        /// <summary>
        /// 显示支付二维码
        /// </summary>
        public bool ShowPayCode { get; set; }
        /// <summary>
        /// 支付二维码自动打印
        /// </summary>
        public bool PayCodeAutoPrint { get; set; }
        /// <summary>
        /// 餐桌ID
        /// </summary>
        public int TableId { get; set; }
        /// <summary>
        /// 餐桌名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 欢迎页图片
        /// </summary>
        public object WelcomeImage { get; set; }
        /// <summary>
        /// 菜品默认图片
        /// </summary>
        public object DishDefaultImage { get; set; }
        /// <summary>
        /// 菜单默认浏览模式
        /// 0：列表模式
        /// 1：无图模式
        /// 2：单图模式
        /// 3：三图模式
        /// </summary>
        public int DefaultBrowseMode { get; set; }
        /// <summary>
        /// 菜单浏览模式（"列表模式"为必选项）
        /// 0：列表模式
        /// 1：无图模式
        /// 2：单图模式
        /// 3：三图模式
        /// </summary>
        public object[] BrowseModes { get; set; }
        /// <summary>
        /// 菜单隐藏分类
        /// </summary>
        public bool HideCategory { get; set; }
        /// <summary>
        /// 呼叫服务
        /// </summary>
        public object[] ServeItems { get; set; }

        /// <summary>
        /// 是否对接系统
        /// </summary>
        public bool IsDocking { get; set; }
    }
}
