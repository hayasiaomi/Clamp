using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Biz
{
    internal class RoleInfo
    {
        //"RoleId":12,"RoleName":"超级管理员","SuperFlag":1

        public int RoleId { set; get; }

        public string RoleName { set; get; }

        public int SuperFlag { set; get; }
    }
}
