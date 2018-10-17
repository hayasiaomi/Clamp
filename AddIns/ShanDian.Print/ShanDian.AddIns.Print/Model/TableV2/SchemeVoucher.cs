using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 门店方案单据
    /// </summary>
    [Table("tb_schemeVoucher")]
    public class SchemeVoucher
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 打印单据名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 打印单据名称描述信息
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 单据编号
        /// 10 点菜单
        /// 11 结账单
        /// 20 支付凭证
        /// 21 退款凭证
        /// 30 一桌一单（厨打总单）
        /// 31 一菜一单（厨打分单）
        /// </summary>
        public int VoucherCode { get; set; }

        /// <summary>
        /// 单据模板
        /// </summary>
        public string TemplateCode { get; set; }

        /// <summary>
        /// 是否启用（0:禁用 1 启用）
        /// </summary>
        public int IsEnabled { get; set; }

        /// <summary>
        /// 打印张数
        /// </summary>
        public int PrintNum { get; set; }

        /// <summary>
        /// 模式(0:无模式 1. 非对接 2. 对接 3. 轻餐)
        /// </summary>
        public int Pattern { get; set; } = 0;

        /// <summary>
        /// 方案ID
        /// </summary>
        public int SchemeId { get; set; }
    }
}
