using System.Collections.Generic;

namespace ShanDian.Machine.Dto.View
{
    public class UserPermissionsVDto
    {
        /// <summary>
        /// 角色列 RolePermissions
        /// </summary>
        public List<RolePermissionsVDto> RolePermissions { get; set; }

        /// <summary>
        /// 拒绝权限列
        /// </summary>
        public List<RefuseDto> RefuseData { get; set; }
    }
}
