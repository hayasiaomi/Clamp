using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Network.Api
{
    internal class SoftwareInfo
    {
        /// <summary>
        /// 当前版本
        /// </summary>
        public string CurrentVersion { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAutoCheckout { set; get; }
        /// <summary>
        /// 餐饮系统名称
        /// </summary>
        public string RestSystem { set; get; }
        /// <summary>
        /// 主机设备IP
        /// </summary>
        public string MainIp { set; get; }
        /// <summary>
        /// 分机数量
        /// </summary>
        public int ExtensionCount { set; get; }
        /// <summary>
        /// 机器码
        /// </summary>
        public string PCID { set; get; }

        /// <summary>
        /// 操作系统
        /// </summary>
        public string OperationSystem { set; get; }

        /// <summary>
        /// 系统位数
        /// </summary>
        public string SystemBit { set; get; }

        public string CPU { set; get; }

        /// <summary>
        /// 内存大小
        /// </summary>
        public string Memory { set; get; }

        /// <summary>
        /// 安装目录
        /// </summary>
        public string InstallFolder { set; get; }

        /// <summary>
        /// 安装目录磁盘剩余空间
        /// </summary>
        public string DiskSpace { set; get; }
    }
}
