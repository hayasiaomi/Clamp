using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    /// <summary>
    /// 预点订单的附加费用
    /// </summary>
    public class KbPreOrderAdditionalCost
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal Price { set; get; }
    }
}
