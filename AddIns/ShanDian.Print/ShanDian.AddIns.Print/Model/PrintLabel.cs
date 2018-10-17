using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    [Table("tb_printLabel")]
    public class PrintLabel
    {
        [ExplicitKey]
        public int Id { get; set; }

        /// <summary>
        /// 标签类型的代号1 餐桌 2菜品
        /// </summary>
        public int LabelGroupCode { get; set; }

        /// <summary>
        ///标签分组Id
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 标签分组名
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        public string LabelName { get; set; }

        /// <summary>
        /// 餐桌ID guid
        /// </summary>
        public string LabelId { get; set; }

    }
}
