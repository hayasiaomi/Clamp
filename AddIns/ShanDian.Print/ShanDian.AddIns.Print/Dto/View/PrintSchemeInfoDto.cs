using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.View
{
    public class PrintSchemeInfoDto
    {
        /// <summary>
        /// 打印方案-包含打印机信息
        /// </summary>
        public PrintSchemeInfoDto()
        {
            TagList = new List<PrintSchemeLabelDto>();
        }
        public int Id { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

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
        /// 标识
        /// </summary>
        public string SchemeCode { get; set; }

        /// <summary>
        /// 选择的打印标签
        /// </summary>
        public List<PrintSchemeLabelDto> TagList { get; set; }

        /// <summary>
        /// 打印机信息
        /// </summary>
        public PrintConfigDto PrintConfig { get; set; }
    }
}
