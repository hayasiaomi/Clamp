using System.Collections.Generic;
using Clamp.Machine.Dto;

namespace Clamp.Machine.Dto.ShanDianView
{
    public class VMRolePermissionIn: VMInput
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 是否管理员角色
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 权限编码列
        /// </summary>
        public List<string> Permissions { get; set; }
    }
}
