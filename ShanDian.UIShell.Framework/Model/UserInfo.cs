using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Model
{
    internal class UserInfo
    {
        public UserInfo()
        {
            this.Permissions = new List<PermissionInfo>();
        }
        public int UserId { set; get; }
        public string UserName { set; get; }
        public string RoleName { set; get; }

        public string Token { set; get; }

        public string Pwd { set; get; }

        public string Sex { set; get; }

        public string Mobile { set; get; }

        public string Status { set; get; }

        public bool IsFirstLogin { set; get; }


        public bool IsAdmin { set; get; }

        public List<PermissionInfo> Permissions { set; get; }

    }

   
}
