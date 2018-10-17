using System.Collections.Generic;
using ShanDian.Machine.Dto;

namespace ShanDian.Machine.Dto.ShanDianView
{
    public class VMUserRolePermissionIn : VMInput
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

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
