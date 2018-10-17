using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    public class LocalPrint.Dto
    {
        public int Id { get; set; }

        /// <summary>
        /// 打印机配置ID
        /// </summary>
        public string PrintId { get; set; }

        /// <summary>
        /// 机器码
        /// </summary>
        public string Machine { get; set; }

        /// <summary>
        /// 打印凭证 张数
        /// </summary>
        public Dictionary<int, int> VoucherList { get; set; }
    }
}
