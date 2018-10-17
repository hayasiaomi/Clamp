using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Clamp.AddIns.Properties;

namespace Clamp.AddIns
{
    /// <summary>
    /// 引用的插件类
    /// </summary>
    public class AddInReference
    {
        /// <summary>
        /// 公司ID
        /// </summary>
        public string ArtifactId { set; get; }

        /// <summary>
        /// 业务组ID
        /// </summary>
        public string GroupId { set; get; }

        /// <summary>
        /// 插件ID
        /// </summary>
        public string AddInId { set; get; }

        /// <summary>
        /// 版本号
        /// </summary>
        public Version Version { set; get; }

        /// <summary>
        /// 是否需要提前加载
        /// </summary>
        public bool RequirePreload { set; get; }

        public AddInReference()
        {

        }


        public AddInReference(string artifactId, string groupId, string addInId, Version version, bool requirePreload)
        {
            this.ArtifactId = artifactId;
            this.GroupId = GroupId;
            this.AddInId = addInId;
            this.Version = version;
            this.RequirePreload = requirePreload;
        }


        public bool Check(List<Bundle> addIns, out Bundle addInFound)
        {
            Bundle addIn = addIns.FirstOrDefault(addin =>
           addin.Manifest.ArtifactId == this.ArtifactId
           && addin.Manifest.GroupId == this.GroupId
           && addin.Manifest.AddInId == this.AddInId
           && addin.Manifest.Version == this.Version);

            addInFound = addIn;

            return addIn != null;
        }
    }
}
