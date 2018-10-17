using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public abstract class PrintData
    {

        public int VoucherId { get; set; }

        /// <summary>
        /// 打印标签（一般为TableID）
        /// </summary>
        public List<string> TagList { get; set; }

        /// <summary>
        /// 本地操作IP（设备id）
        /// </summary>
        public string PcId { get; set; }

        /// <summary>
        /// 来源
        /// 1. 米客(线上)
        /// 2. 系统(线下)
        /// （用于识别tableid，dishtypeid的类型是米客ID还是erpid）
        /// </summary>
        public int OriginCode { get; set; } = 1;
    }
}
