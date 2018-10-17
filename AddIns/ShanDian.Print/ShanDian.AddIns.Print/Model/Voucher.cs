using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 打印凭证模版
    /// </summary>
    [Table("tb_voucher")]
    public class Voucher
    {
        [ExplicitKey]
        public int Id { get; set; }

        /// <summary>
        /// 凭证名称
        /// </summary>
        public string VoucherName { get; set; }

        /// <summary>
        /// 模板编号
        /// </summary>
        public string TemplateCode { get; set; }

        /// <summary>
        /// 支持的分组标签ID
        /// </summary>
        public string GroupCode { get; set; }

        /// <summary>
        /// 凭证是否开启打印
        /// </summary>
        public bool Enble { get; set; }

        /// <summary>
        /// 是否启用分组打印
        /// </summary>
        public bool Overall { get; set; }

        /// <summary>
        /// 是否支持本地
        /// </summary>
        public bool LocalVoucher { get; set; }

        /// <summary>
        /// 是否全局打印凭证
        /// </summary>
        public bool GlobalVoucher { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 模板路径
        /// </summary>
        public string Path { get; set; }
    }
}
