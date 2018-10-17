using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanDain.AIM.DTO
{
    public class AddInInfo
    {
        /// <summary>
        /// 展示名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 插件作者
        /// </summary>
        public virtual string Auther { get; set; }
        /// <summary>
        /// 版权信息
        /// </summary>
        public virtual string Copyright { get; set; }
        /// <summary>
        /// 插件描述信息
        /// </summary>
        public virtual string Description { get; set; }

        public virtual string ArtifactId { get; set; }
        /// <summary>
        /// 组织ID
        /// </summary>
        public virtual string GroupId { get; set; }
        /// <summary>
        /// 插件ID(用作唯一标识)
        /// </summary>
        public virtual string AddInId { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public virtual VersionInfo Version { get; set; }
        /// <summary>
        /// 依赖列表
        /// </summary>
        public List<RefAddInInfo> Dependency { get; set; } = new List<RefAddInInfo>();
        /// <summary>
        /// 冲突列表
        /// </summary>
        public List<RefAddInInfo> Conflict { get; set; } = new List<RefAddInInfo>();
        /// <summary>
        /// 校验和
        /// </summary>
        public string CheckSum { get; set; }

        public override bool Equals(object obj)
        {
            var temp = obj as AddInInfo;
            if (temp == null)
                return false;
            return temp.AddInId == temp.AddInId && temp.Version == this.Version;
        }
    }
}
