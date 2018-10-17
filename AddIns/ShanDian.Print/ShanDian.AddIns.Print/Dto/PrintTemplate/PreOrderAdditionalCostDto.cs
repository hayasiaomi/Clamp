using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintTemplate
{
    public class PreOrderAdditionalCostDto
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
