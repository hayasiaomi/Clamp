using ShanDian.Machine.Dto;

namespace ShanDian.Machine.Dto.View
{
    public class RoleUserInDto:InputDto
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 用户Id组
        /// </summary>
        public int[] UserIds { get; set; }
    }
}
