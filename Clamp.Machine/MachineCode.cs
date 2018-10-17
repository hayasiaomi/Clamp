using Clamp.Software.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Machine
{
    public class MachineCode : SystemError<MachineCode>
    {
        /// <summary>
        /// 违规操作
        /// </summary>
        public int IllegalOperationError = 2001;

        /// <summary>
        /// 结果为空
        /// </summary>
        public int ResultNull = 2002;

        /// <summary>
        /// 数据更新失败
        /// </summary>
        public int UpdateError = 2003;

        /// <summary>
        /// 重复添加
        /// </summary>
        public int RepeatedError = 2004;
    }
}
