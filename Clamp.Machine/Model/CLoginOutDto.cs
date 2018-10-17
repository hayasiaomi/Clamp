using System.Collections.Generic;

namespace Clamp.Machine.Model
{
    public class CLoginOutDto
    {

        /// <summary>
        /// 用户Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户账户（工号）
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 是否系统默认管理员
        /// </summary>
        public bool IsDefaultAdmin { get; set; } = false;

        /// <summary>
        /// 是否账号首次登录使用
        /// </summary>
        public bool IsFirstLogin { get; set; } = false;

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 用户所有权限
        /// </summary>
        public List<CPermissionCategoryDto> Permissions { get; set; }


    }
}
