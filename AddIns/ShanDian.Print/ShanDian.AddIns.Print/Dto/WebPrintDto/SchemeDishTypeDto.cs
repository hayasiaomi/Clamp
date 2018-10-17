using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.WebPrint.Dto
{
    public class SchemeDishTypeDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 打印方案Id
        /// </summary>
        public int SchemeId { get; set; }

        /// <summary>
        /// 线上分类ID
        /// </summary>
        public string MKDishTypeID { get; set; }

        /// <summary>
        /// 线下分类ID
        /// </summary>
        public string ErpDishTypeID { get; set; }

        /// <summary>
        /// 菜品分类名称
        /// </summary>
        public string DishTypeName { get; set; }
    }
}
