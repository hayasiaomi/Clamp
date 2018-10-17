namespace ShanDian.Machine.Dto.View
{
    public class GrantUserVDto
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
        /// 管理员（0 不是 1是） 
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public int SuperFlag { get; set; }

        /// <summary>
        /// 状态（0停用 1启用）
        /// </summary>
        public int Status { get; set; }
    }
}
