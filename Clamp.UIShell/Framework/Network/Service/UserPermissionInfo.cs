using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Network.Service
{
    internal class UserPermissionInfo
    {
        /// <summary>
        /// 角色列 RolePermissions
        /// </summary>
        public List<RolePermissionInfo> RolePermissions { get; set; }

        /// <summary>
        /// 拒绝权限列
        /// </summary>
        public List<RefuseInfo> RefuseData { get; set; }
    }
}
