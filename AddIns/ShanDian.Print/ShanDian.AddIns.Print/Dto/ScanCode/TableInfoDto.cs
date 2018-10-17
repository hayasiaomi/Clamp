using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.ScanCode
{
    public class TableInfoDto
    {
        public List<TableInfo> tableInfos { get; set; }
    }
    public class TableInfo
    {
        /// <summary>
        /// 线上餐桌ID
        /// </summary>
        public string MKTableID { get; set; }

        /// <summary>
        /// 线下餐桌ID
        /// </summary>
        public string ErpTableID { get; set; }

        /// <summary>
        /// 餐桌名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 线下区域ID
        /// </summary>
        public string ErpAreaID { get; set; }

        /// <summary>
        /// 区域名稱
        /// </summary>
        public string AreaName { get; set; }
    }
}
