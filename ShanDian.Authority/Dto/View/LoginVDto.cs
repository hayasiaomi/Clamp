using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Machine.Dto.View
{
    /// <summary>
    /// 登录验证返回Token
    /// </summary>
    public class LoginVDto
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 登录返回Token
        /// </summary>
        public string Token { get; set; }
    }
}
