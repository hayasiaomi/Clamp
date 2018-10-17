using System.Collections.Generic;

namespace ShanDian.Machine.Dto.View
{
    public class UserRoleVDto: UserVDto
    {
        /// <summary>
        /// 用户列
        /// </summary>
        public List<RoleDto> RoleData { get; set; }
    }
}
