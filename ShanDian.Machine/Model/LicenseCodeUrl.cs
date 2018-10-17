

namespace ShanDian.Machine.Model
{
    public class LicenseCodeUrl:LicenseCode
    {
        /// <summary>
        /// 请求权限的用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 授权人Id
        /// </summary>
        public int GrantId { get; set; }

        /// <summary>
        /// 授权人姓名
        /// </summary>
        public string GrantName { get; set; }

        /// <summary>
        /// 请求权限编码
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// 请求权限url
        /// </summary>
        public string Url { get; set; }
      
    }
}
