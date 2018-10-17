namespace ShanDian.Machine.Dto.View
{
    /// <summary>
    /// 用户基本信息
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///员工编码
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; } = 0;

        /// <summary>
        /// 手机号
        /// </summary>

        public string Mobile { get; set; }


        /// <summary>
        /// 状态（0停用 1启用）
        /// </summary>
        public int Status { get; set; } = 1;

        /// <summary>
        /// 管理员角色（0 不是 1是）
        /// </summary>
        public int SuperFlag { get; set; } = 0;

    }
}
