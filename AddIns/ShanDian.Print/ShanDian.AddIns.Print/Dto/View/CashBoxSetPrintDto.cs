using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.View
{
    public class CashBoxSetPrint.Dto
    {
        public CashBoxSetPrint.Dto()
        {
            IsSet = 0;
        }
        /// <summary>
        /// 打印机ID
        /// </summary>
        public string PrintId { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }

        /// <summary>
        /// 已被设置(使用)
        /// </summary>
        public int IsSet { get; set; }
    }
}
