

namespace Clamp.Machine.Model
{
    public class LicenseCodePwd: LicenseCode
    {

        /// <summary>
        /// 修改用户密码的用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 操作人Id
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// 操作人姓名
        /// </summary>
        public string OperatorName { get; set; }




    }
}
