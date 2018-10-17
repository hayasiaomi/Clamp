using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper.Contrib.Extensions;

namespace ShanDian.AddIns.Print.Model
{
    /// <summary>
    /// 钱箱设置
    /// </summary>
    [Table("tb_cashBoxSetInfo")]
    public class CashBoxSetInfo
    {
        public int Id { get; set; }

        /// <summary>
        /// 终端Id
        /// </summary>
        public string TerminaId { get; set; }

        /// <summary>
        /// 终端名称
        /// </summary>
        public string TerminalName { get; set; }

        /// <summary>
        /// 打印机Id
        /// </summary>
        public string PrintId { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }

        /// <summary>
        /// 是否已设置
        /// </summary>
        public int IsSet { get; set; }
    }
}
