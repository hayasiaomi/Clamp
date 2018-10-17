using System.Collections.Generic;
using Clamp.Machine.Dto;

namespace Clamp.Machine.Model
{
    public class CUserRolePermissionInDto: VMInput
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限编码列
        /// </summary>
        public List<string> PermissionList { get; set; }
    }
}
