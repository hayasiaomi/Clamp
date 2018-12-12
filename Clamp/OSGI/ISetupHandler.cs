using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI
{
    /// <summary>
    /// 安装处理者类
    /// </summary>
    internal interface ISetupHandler
    {
        /// <summary>
        /// 检测指定路径
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="scanFolder"></param>
        /// <param name="filesToIgnore"></param>
        void Scan(BundleRegistry registry, string scanFolder, string[] filesToIgnore);

        void GetBundleDescription(BundleRegistry registry, string file, string outFile);
    }
}
