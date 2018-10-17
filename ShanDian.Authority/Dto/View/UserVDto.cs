namespace ShanDian.Machine.Dto.View
{
    public class UserVDto
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
        /// 状态（0停用 1启用）
        /// </summary>
        public int Status { get; set; }
    }
}
