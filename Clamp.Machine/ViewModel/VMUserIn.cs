using System.Collections.Generic;
using Clamp.Machine.Dto;

namespace Clamp.Machine.Dto.ShanDianView
{
    public class VMUserIn : VMInput
    {
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
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 权限列
        /// </summary>
        public List<string> PermissionList { get; set; }
    }
}
