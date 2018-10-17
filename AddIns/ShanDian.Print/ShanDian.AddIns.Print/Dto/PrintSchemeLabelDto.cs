using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    /// <summary>
    /// 打印机标签
    /// </summary>
    public class PrintSchemeLabelDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 打印方案Id
        /// </summary>
        public int PrintSchemeId { get; set; }

        /// <summary>
        /// 标签类型的代号1 餐桌 2菜品
        /// </summary>
        public int LabelGroupCode { get; set; }

        /// <summary>
        /// 餐桌ID guid
        /// </summary>
        public string LabelId { get; set; }

        /// <summary>
        /// 餐桌名
        /// </summary>
        public string LabelName { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string SchemeCode { get; set; }
    }
}
