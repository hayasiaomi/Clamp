using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Biz
{
    internal class PermissionInfo
    {
        /// <summary>
        /// 权限id
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// 权限编码（权限固定标识）
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// 权限父级ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// 权限路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 权限编码 1、2、4、8 增删改查 满权限为15 F
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 功能类型（1功能导航 2 设置导航 3 操作控制）
        /// </summary>
        public int PermissionType { get; set; }

        /// <summary>
        /// 是否显示（0不显示 1显示）
        /// </summary>
        public int IsEnabled { get; set; }
    }


    internal class PermissionInfoComparer : IEqualityComparer<PermissionInfo>
    {
        public bool Equals(PermissionInfo x, PermissionInfo y)
        {
            return x.PermissionCode == x.PermissionCode;
        }

        public int GetHashCode(PermissionInfo obj)
        {
            return obj.PermissionCode.GetHashCode();
        }
    }
}
