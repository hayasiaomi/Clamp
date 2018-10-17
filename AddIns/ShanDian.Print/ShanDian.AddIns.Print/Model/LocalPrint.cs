using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 本地打印方案
    /// </summary>
    [Table("tb_localPrint")]
    public class LocalPrint
    {
        [ExplicitKey]
        public int Id { get; set; }

        /// <summary>
        /// 打印机配置ID
        /// </summary>
        public string PrintId { get; set; }

        /// <summary>
        /// 机器码
        /// </summary>
        public string Machine { get; set; }
    }
}
