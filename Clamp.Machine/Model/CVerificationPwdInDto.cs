namespace Clamp.Machine.Model
{
    /// <summary>
    /// 入参：验证用户密码
    /// </summary>
    public class CVerificationPwdInDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
    }
}
