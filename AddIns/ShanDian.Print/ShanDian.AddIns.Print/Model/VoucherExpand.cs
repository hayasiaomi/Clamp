using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 单据拓展
    /// </summary>
    [Table("tb_voucherExpand")]
    public class VoucherExpand
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        public string TemplateCode { get; set; }

        public int Type { get; set; }

        public int Alignment { get; set; }

        public string Content { get; set; }
    }
}
