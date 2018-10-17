namespace ShanDian.Machine.Model
{
    public class CPermissionDto
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 权限访问Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 权限排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 平台组件icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 是否善点内部权限 true 、false
        /// </summary>
        public bool IsInner { get; set; } = true;

        /// <summary>
        /// 平台组件需要的
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 是否有高级权限
        /// </summary>
        public bool IsMoreConfig { get; set; } = false;

        /// <summary>
        /// 高级权限配置URL
        /// </summary>
        public string ConfigUrl { get; set; }

    }
}
