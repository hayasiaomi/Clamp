using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    /// <summary>
    /// 打印方案
    /// </summary>
    public class PrintSchemeDto
    {
        public PrintSchemeDto()
        {
            TagList = new List<PrintSchemeLabelDto>();
        }
        public int Id { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 打印机配置ID
        /// </summary>
        public string PrintId { get; set; }

        /// <summary>
        /// 打印模版ID
        /// </summary>
        public int VoucherId { get; set; }

        /// <summary>
        ///  0本地打印 1为分组默认打印机 2分组打印
        /// </summary>
        public int LocalMachine { get; set; }

        /// <summary>
        /// 本地机器ID
        /// </summary>
        public string PcId { get; set; }

        /// <summary>
        /// 打印数量
        /// </summary>
        public int PrintNum { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string SchemeCode { get; set; }

        /// <summary>
        /// 选择的打印标签
        /// </summary>
        public List<PrintSchemeLabelDto> TagList { get; set; }
    }
}
