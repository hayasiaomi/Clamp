namespace ShanDian.Machine.Model
{
    public class CSimpleUserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 是否系统默认管理员
        /// </summary>
        public bool IsDefaultAdmin { get; set; } = false;
    }
}
