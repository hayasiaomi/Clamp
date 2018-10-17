using System.Collections.Generic;
using ShanDian.Machine.Dto;

namespace ShanDian.Machine.Dto.View
{
     public class UserRoleRefuseInDto: InputDto
     {
         /// <summary>
         /// Id
         /// </summary>
         public int UserId { get; set; } = 0;

        /// <summary>
        ///名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///员工编码
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }

         /// <summary>
         /// 性别
         /// </summary>
         public int Sex { get; set; } = 0;

        /// <summary>
        /// 手机号
        /// </summary>

        public string Mobile { get; set; }


         /// <summary>
         /// 状态（0停用 1启用）
         /// </summary>
         public int Status { get; set; } = 1;

        /// <summary>
        /// 角色列表
        /// </summary>
        public List<RoleDto> RoleData { get; set; }

        /// <summary>
        /// 拒绝权限列表
        /// </summary>
        public List<RefuseDto> RefuseData { get; set; }
    }
}
