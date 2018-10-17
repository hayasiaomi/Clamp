using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class PrintFailVoucher : PrintData
    {
        /// <summary>
        /// 系统桌号
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 系统单号
        /// </summary>
        public string BillNo { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayPattern { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public string PayMoney { get; set; }
        /// <summary>
        /// 支付凭证
        /// </summary>
        public string OrderPayNo { get; set; }
    }
}
