using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Model
{
    public class MsgBubbleNotice
    {
        public MsgBubbleNotice()
        {
            this.CreateDate = DateTime.Now;
            this.IsDispose = false;
        }
        /// <summary>
        /// ID唯一标识
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 数量
        /// </summary>
        public int TotalNum { set; get; }

        /// <summary>
        /// 是否处理过
        /// </summary>
        public bool IsDispose { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
    }
}
