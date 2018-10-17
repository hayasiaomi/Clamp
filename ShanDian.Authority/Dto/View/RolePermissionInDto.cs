using System.Collections.Generic;
using ShanDian.Machine.Dto;

namespace ShanDian.Machine.Dto.View
{
    /// <summary>
    /// 角色信息与角色下的权限Id集合
    /// </summary>
    public class RolePermissionInDto : InputDto
    {

        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; } = 0;

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 管理员角色（0 不是 1是）
        /// </summary>
        public int SuperFlag { get; set; } = 0;

        ///// <summary>
        ///// 权限列
        ///// </summary>
        //public int[] PermissionIds { get; set; }

        /// <summary>
        /// 权限列
        /// </summary>
        public List<int> PermissionIds { get; set; }
    }

    public class Permission
    {
        /// <summary>
        /// 权限Id
        /// </summary>
        public int PermissionId { get; set; }
    }

}
