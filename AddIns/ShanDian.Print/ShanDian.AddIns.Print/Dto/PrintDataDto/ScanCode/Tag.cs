using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto.PrintDataDto
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string TagName { get; set; }

        public string OrderDishId { get; set; }

        /// <summary>
        /// 类型 （0：分量   1：做法）
        /// </summary>
        public int Type { get; set; }
    }
}

