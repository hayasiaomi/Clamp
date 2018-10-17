using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 暂不使用
    /// </summary>
    [Table("tb_voucherLabel")]
    public class VoucherLabel
    {
        [ExplicitKey]
        public int Id { get; set; }

        /// <summary>
        /// 标签的编号
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// 打印方案的编号
        /// </summary>
        public int SchemeId { get; set; }


    }
}
