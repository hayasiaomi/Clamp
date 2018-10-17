using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    /// <summary>
    /// 打印凭证模版
    /// </summary>
    public class VoucherDto
    {
        public VoucherDto()
        {
            PrintNum = 0;
            SchemeNum = 0;
        }

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
        /// 支持的标签ID
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
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 打印数量
        /// </summary>
        public int PrintNum { get; set; }

        /// <summary>
        /// 方案数量
        /// </summary>
        public int SchemeNum { get; set; }

        /// <summary>
        /// 模板路径
        /// </summary>
        public string Path { get; set; }
    }
}
