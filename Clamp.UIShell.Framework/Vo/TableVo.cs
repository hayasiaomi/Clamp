using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Vo
{
    /// <summary>
    /// 前面小写是因为了跟前端同步
    /// </summary>
    public class TableVo
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public string areaId { set; get; }
        /// <summary>
        /// 餐桌ID
        /// </summary>
        public string tableId { set; get; }
        /// <summary>
        /// 餐桌号
        /// </summary>
        public string tableName { set; get; }

    }
}
