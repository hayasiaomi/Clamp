using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    /// <summary>
    /// 激活类的接口
    /// </summary>
    public interface IBundleActivator
    {
        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="addInContext"></param>
        void Start(BundleContext addInContext);

        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="addInContext"></param>
        void Stop(BundleContext addInContext);
    }
}
