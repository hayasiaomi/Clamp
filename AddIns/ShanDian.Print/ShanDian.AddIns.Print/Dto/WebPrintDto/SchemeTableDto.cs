using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.WebPrint.Dto
{
    public class SchemeTableDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 打印方案Id
        /// </summary>
        public int SchemeId { get; set; }

        /// <summary>
        /// 线上餐桌ID
        /// </summary>
        public string MKTableID { get; set; }

        /// <summary>
        /// 线下餐桌ID 
        /// </summary>
        public string ErpTableID { get; set; }

        /// <summary>
        /// 线下餐桌ID 
        /// </summary>
        public string ErpTableAreaID { get; set; }

        /// <summary>
        /// 餐桌名
        /// </summary>
        public string TableName { get; set; }
    }
}
