using System.Collections.Generic;

namespace ShanDian.Machine.Dto.ShanDianView
{
    /// <summary>
    /// 登录验证返回Token
    /// </summary>
    public class VMLogin
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
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 是否默认系统管理员
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 是否账号首次登录使用
        /// </summary>
        public bool IsFirstLogin { get; set; } = false;

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 权限列表
        /// </summary>
        public List<VMPermission> Permissions { get; set; }

        /// <summary>
        /// 登录返回Token
        /// </summary>
        public string Token { get; set; }
    }
}
