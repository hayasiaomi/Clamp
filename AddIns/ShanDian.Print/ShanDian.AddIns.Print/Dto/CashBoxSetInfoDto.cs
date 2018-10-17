using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    public class CashBoxSetInfoDto
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
