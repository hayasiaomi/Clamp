using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Biz
{
    internal class CoreUserPermission
    {
        public int ParentId { set; get; }

        public string PermissionCode { set; get; }

        public int PermissionId { set; get; }

        public string PermissionName { set; get; }
    }
}
