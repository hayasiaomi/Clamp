using System;

namespace ShanDian.Machine.Model
{
    public class LicenseCode
    {
        /// <summary>
        /// 授权码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime DeadlineTime { get; set; }
    }
}
