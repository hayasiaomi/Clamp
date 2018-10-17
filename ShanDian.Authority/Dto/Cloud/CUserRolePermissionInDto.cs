using System.Collections.Generic;
using ShanDian.Machine.Dto;

namespace ShanDian.Machine.Model
{
    public class CUserRolePermissionInDto: InputDto
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
