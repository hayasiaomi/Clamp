using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print.Dto
{
    public class PrintInfoDto
    {
        private string printId = Guid.NewGuid().ToString("N");
        private int state = 1;

        /// <summary>
        /// 主键
        /// </summary>
        public string PrintId { get { return printId; } set { printId = value; } }

        /// <summary>
        /// Pcid
        /// </summary>       
        public string Pcid { get; set; }

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }

        /// <summary>
        /// 打印机状态（0：异常   1：正常）
        /// </summary>
        public int State { get { return state; } set { state = value; } }
    }
}
