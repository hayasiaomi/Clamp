using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    public class VoucherExpandDto
    {
        public VoucherExpandDto()
        {
            TemplateCode = string.Empty;
            Content = string.Empty;
        }
        public int Id { get; set; }

        /// <summary>
        /// 模板编号
        /// </summary>
        public string TemplateCode { get; set; }

        /// <summary>
        /// 类型 （0：页头   1：页尾  10:支付凭证是否显示菜品  11:支付凭证是否显示菜品金额）
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 对齐 （0：左对齐   1：居中  2：右对齐）
        /// </summary>
        public int Alignment { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
