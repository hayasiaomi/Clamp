using System.Collections.Generic;

namespace ShanDian.Machine.Dto.View
{
    public class RolePermissionsVDto
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 权限列
        /// </summary>
        public List<PermissionDto> PermissionData { get; set; }
    }
}
