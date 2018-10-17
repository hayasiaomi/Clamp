using ShanDian.Machine.Dto;

namespace ShanDian.Machine.Model
{
    public class CUserInfoInDto: InputDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户账户（工号）
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        public bool Status { get; set; }


    }
}
