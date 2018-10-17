using System;

namespace Clamp.Machine.Dto.ShanDianView
{
    public class VMUserSummary
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
        /// 启用状态
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 是否默认管理员
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }


    }
}
