using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns
{
    /// <summary>
    /// 激活类的接口
    /// </summary>
    public interface IAddInActivator
    {
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="addInContext"></param>
        void Start(AddInContext addInContext);

        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="addInContext"></param>
        void Stop(AddInContext addInContext);
    }
}
