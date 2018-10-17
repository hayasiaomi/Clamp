using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Server
{
    public class ServerProgressInfo
    {
        //{"Complete":true,"Message":["数据总线完成","产品组件加载","配置数据下载","帐号权限下载","餐桌菜品下载"],"PartsNumber":10,"CompleteNumber":10}

        public bool Complete { set; get; }

        public List<string> Message { get; set; }

        public int PartsNumber { get; set; }

        public int CompleteNumber { get; set; }
    }
}
