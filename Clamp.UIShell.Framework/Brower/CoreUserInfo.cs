using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Brower
{
    public class CoreUserInfo
    {
        public CoreUserInfo()
        {
            this.Permissions = new List<CoreUserPermission>();
        }

        public int UserId { set; get; }
        public string UserName { set; get; }
        public string RoleName { set; get; }

        public string Token { set; get; }

        public string Pwd { set; get; }

        public string Sex { set; get; }

        public string Mobile { set; get; }
        public string Status { set; get; }
        public bool IsAdmin { get; set; }

        public bool IsFirst { set; get; }
        public List<CoreUserPermission> Permissions { set; get; }
    }
}
