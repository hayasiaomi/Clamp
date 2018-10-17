using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    /// <summary>
    /// 扫码点菜单的支付方式类
    /// </summary>
    public class SmdcdPayTypeAmountDto
    {
        /// <summary>
        /// 支付名称
        /// </summary>
        public string PayTypeName { set; get; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Price { set; get; }


    }
}
