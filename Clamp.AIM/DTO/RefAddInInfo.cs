using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.AIM.DTO
{
    /// <summary>
    /// 引用其他插件的信息
    /// </summary>
    public class RefAddInInfo
    {
        public string ArtifactId { get; set; }
        /// <summary>
        /// 组织ID
        /// </summary>
        public string GroupId { get; set; }
        /// <summary>
        /// 引用插件的ID
        /// </summary>
        public string AddInId { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public VersionInfo Version { get; set; }

        public RefAddInInfo(string artifactid, string groupid, string addinid, VersionInfo version)
        {
            this.ArtifactId = artifactid;
            this.GroupId = groupid;
            this.AddInId = addinid;
            this.Version = version;
        }
    }
}
