using System.Collections.Generic;
using ShanDian.Machine.Dto;

namespace ShanDian.Machine.Model
{
    public class CRolePermissionInDto : VMInput
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }

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
