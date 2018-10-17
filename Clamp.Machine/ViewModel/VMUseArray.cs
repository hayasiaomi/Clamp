using System.Collections.Generic;
using Clamp.Machine.Dto;

namespace Clamp.Machine.Dto.ShanDianView
{
    public class VMUseArray : VMInput
    {
        /// <summary>
        /// 角色Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 用户Id列
        /// </summary>
        public List<int> UserIds { get; set; }
    }
}
