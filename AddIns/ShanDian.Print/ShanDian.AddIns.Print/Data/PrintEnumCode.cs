using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Data
{
    /// <summary>
    /// 打印订单的类型编号
    /// </summary>
    public class PrintEnumCode
    {
        private int BillCode;
        public PrintEnumCode()
        {
            //扫码点菜单默认单据类型编号为100
            BillCode = 100;
        }
        /// <summary>
        /// 单据类型
        /// </summary>
        public enum CheckBillCode : int
        {
            [System.ComponentModel.Description("扫码点菜单")]
            SMdcd = 100,
            [System.ComponentModel.Description("整单下单失败单")]
            XDsbd = 101,
            [System.ComponentModel.Description("扫码点菜结账单")]
            SMjzd = 102,
            [System.ComponentModel.Description("轻餐版专用结账单")]
            QCjzd = 103
        }

        public enum TableTypeCode : int
        {
            [System.ComponentModel.Description("桌牌号")]
            ZPH = 1,
            [System.ComponentModel.Description("取餐号")]
            QCH = 2,
        }

        public enum PrintGroupCode : int
        {
            [System.ComponentModel.Description("点菜打印方案")]
            PrtOrder = 10,
            [System.ComponentModel.Description("支付打印方案")]
            PrtPayment = 20,
            [System.ComponentModel.Description("后厨打印方案")]
            PrtKitchen = 30,
            [System.ComponentModel.Description("预点打印方案")]
            PrtPreOrder = 40,
            [System.ComponentModel.Description("外卖打印方案")]
            PrtTKOrder = 50,
        }

        public enum OriginCode : int
        {
            [System.ComponentModel.Description("米客")]
            MK = 1,
            [System.ComponentModel.Description("系统")]
            ERP = 2,
        }

        public enum DishTypeOriginCode : int
        {
            [System.ComponentModel.Description("米客菜品分类")]
            MK = 1,
            [System.ComponentModel.Description("系统菜品分类")]
            ERP = 2,
        }

        public enum AreaOriginCode : int
        {
            [System.ComponentModel.Description("餐桌分区")]
            Table = 1,
            [System.ComponentModel.Description("菜品分区")]
            DishType = 2,
        }

        public enum SetInfoKey : int
        {
            [System.ComponentModel.Description("通用--没有配置信息")]
            Normal = 0,
            [System.ComponentModel.Description("下单失败单")]
            FailOrder = 101,
            [System.ComponentModel.Description("自动核销失败单")]
            PayFail = 102,
        }

        public enum VoucherCode : int
        {
            [System.ComponentModel.Description("点菜单")]
            Order = 10,

            [System.ComponentModel.Description("结账单")]
            ChechOut = 11,

            [System.ComponentModel.Description("支付凭证")]
            Payment = 20,

            [System.ComponentModel.Description("退款凭证")]
            Refund = 21,

            [System.ComponentModel.Description("一桌一单")]
            ByDesk = 30,

            [System.ComponentModel.Description("一菜一单")]
            ByDish = 31,

            [System.ComponentModel.Description("预点单")]
            PreOrder = 40,

            [System.ComponentModel.Description("外卖单")]
            TKout = 50,
        }

        public enum DishTypeCode : int
        {
            [System.ComponentModel.Description("菜品分类")]
            Sort = 0,
            [System.ComponentModel.Description("菜品销量")]
            Sale = 1,
            [System.ComponentModel.Description("退菜数量")]
            Back = 2,
        }

        public enum TKOrderCode:int
        {
            [System.ComponentModel.Description("下单成功")]
            TK1 = 101,
            [System.ComponentModel.Description("下单失败")]
            TK2 = 102,
        }
    }
}
