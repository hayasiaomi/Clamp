using System.Collections.Generic;

namespace ShanDian.Machine.Dto.View
{
    public class UserInfoVDto: UserDto
    {
        /// <summary>
        /// 用户列
        /// </summary>
        public List<RoleDto> RoleData { get; set; }
    }
}
