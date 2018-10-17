namespace ShanDian.Machine.Dto.ShanDianView
{
    public class RoleSummaryDto
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 是否管理员角色
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 用户数
        /// </summary>
        public int UserCount { get; set; }
    }
}
