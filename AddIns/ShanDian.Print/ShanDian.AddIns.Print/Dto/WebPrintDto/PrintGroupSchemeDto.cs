using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.WebPrint.Dto
{
    public class PrintGroupSchemeDto
    {
        public PrintGroupSchemeDto()
        {
            DishTypeCassify = new List<SchemeDishTypeDto>();
            TableClassify = new List<SchemeTableDto>();
            VoucherList = new List<SchemeVoucherDto>();
            SetInfoList = new List<PrintSetInfoDto>();
        }

        public int Id { get; set; }

        /// <summary>
        /// Pcid
        /// </summary>
        public string Pcid { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 打印机配置ID
        /// </summary>
        public string PrintId { get; set; }

        /// <summary>
        /// 是否默认（1 默认 0 不默认）
        /// </summary>
        public int IsDefault { get; set; }

        /// <summary>
        /// 打印方案状态（0:禁用 1 启用）
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 方案组对应ID
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 方案创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 方案修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 菜品分区
        /// </summary>
        public List<SchemeDishTypeDto> DishTypeCassify { get; set; }

        /// <summary>
        /// 餐桌分区
        /// </summary>
        public List<SchemeTableDto> TableClassify { get; set; }

        /// <summary>
        /// 前台单据显示
        /// </summary>
        public List<SchemeVoucherDto> VoucherList { get; set; }

        /// <summary>
        /// 前台额外配置显示（打印方案相关）
        /// </summary>
        public List<PrintSetInfoDto> SetInfoList { get; set; }
    }
}
