using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Model
{
    public class Notice
    {
        public Notice()
        {
            this.CreateDate = DateTime.Now;
            this.IsDisplay = false;
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string IconName { set; get; }

        /// <summary>
        /// 消息ID
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 消息编号
        /// </summary>
        public string SerialNumber { set; get; }

        /// <summary>
        /// 消息类型 10：下单消息 11：支付消息 12：扫码提醒 13：服务提醒 14：系统消息
        /// </summary>
        public NoticeCategory Category { set; get; }
        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public string Content { set; get; }
        /// <summary>
        /// 关闭的秒。如果为负就手动关闭
        /// </summary>
        public int ShutCount { set; get; }
        /// <summary>
        /// 是否读取显示过
        /// </summary>
        public bool IsDisplay { set; get; }

        /// <summary>
        /// 查看详情的URL	
        /// </summary>
        public string UrlString { set; get; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
    }
}
