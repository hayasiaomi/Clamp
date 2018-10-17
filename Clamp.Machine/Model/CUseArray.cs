using System.Collections.Generic;
using Clamp.Machine.Dto;

namespace Clamp.Machine.Model
{
    public class CUseArray: VMInput
    {
        /// <summary>
        /// 用户Id列
        /// </summary>
        public List<int> Users { get; set; }
    }
}
