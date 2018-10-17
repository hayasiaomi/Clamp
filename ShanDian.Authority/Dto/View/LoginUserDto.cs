using System;
using ShanDian.Machine.Dto.View;

namespace ShanDian.Machine.Dto.View
{
    /// <summary>
    /// 云平台用户登录返回用户基本信息
    /// </summary>
    public class LoginUserDto: UserDto
    {

        /// <summary>
        /// 最后登录时间
        /// </summary>
         public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
         public string LastLoginIp { get; set; }

    }
}
