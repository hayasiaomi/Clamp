using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Biz
{
    internal class CoreUserInfo
    {
        public CoreUserInfo()
        {
            this.permissions = new List<CoreUserPermission>();
        }

        public string UserId { set; get; }
        public string UserName { set; get; }
        public string Pwd { set; get; }
        public string Token { set; get; }
        public List<CoreUserPermission> permissions { set; get; }
    }
}
