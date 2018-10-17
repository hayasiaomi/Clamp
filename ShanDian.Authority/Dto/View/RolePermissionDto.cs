namespace ShanDian.Machine.Dto.View
{
    public class RolePermissionDto
    {
        public RolePermissionDto()
        {
            RoleId = 0;
            SuperFlag = 0;
        }
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 管理员角色（0 不是 1是）
        /// </summary>
        public int SuperFlag { get; set; }

        /// <summary>
        /// 权限列
        /// </summary>
        public int[] PermissionIds { get; set; }
    }
}
