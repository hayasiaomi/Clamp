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
        /// <param name="context"></param>
        void Start(IBundleContext context);

        /// <summary>
        /// 结束
        /// </summary>
        /// <param name="context"></param>
        void Stop(IBundleContext context);
    }
}
