using System.Collections.Generic;

namespace Clamp.Machine.Dto.ShanDianView
{
    public class VMUserDetail
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户账户（工号）
        /// </summary>
        public string UserCode { get; set; }

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
        /// 是否默认系统管理员
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<VMSimplePermission> Permissions { get; set; }

    }
}
