using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data
{
    /// <summary>
    /// Bundle的状态
    /// </summary>
    [Flags]
    public enum BundleFlags
    {
        None = 0,
        /// <summary>
        /// 不能安装
        /// </summary>
        CantUninstall = 1,
        /// <summary>
        /// 不可用
        /// </summary>
        CantDisable = 2,
        /// <summary>
        /// 隐藏
        /// </summary>
        Hidden = 4
    }
}
