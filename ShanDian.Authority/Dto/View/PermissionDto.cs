namespace ShanDian.Machine.Dto.View
{
    /// <summary>
    /// 权限基础信息
    /// </summary>
    public class PermissionDto
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

        ///// <summary>
        ///// 系统标识（0 不是 1是）
        ///// </summary>
        //public int SystemFlag { get; set; }

        ///// <summary>
        ///// 排序
        ///// </summary>
        //public int sortIndex { get; set; }

    }
}
