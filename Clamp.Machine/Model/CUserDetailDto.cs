using System.Collections.Generic;

namespace Clamp.Machine.Model
{
    public class CUserDetailDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户账户（工号）
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 是否系统默认管理员
        /// </summary>
        public bool IsDefaultAdmin { get; set; } = false;

        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 用户所有权限
        /// </summary>
        public List<CPermissionCategoryDto> Permissions { get; set; }
    }
}
