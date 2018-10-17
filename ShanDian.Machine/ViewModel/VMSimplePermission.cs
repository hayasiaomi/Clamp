namespace ShanDian.Machine.Dto.ShanDianView
{
    public class VMSimplePermission
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
        /// 权限排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 平台组件icon
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 权限功能分类
        /// </summary>
        public string KindCode { get; set; }

        /// <summary>
        /// 父级权限Code
        /// </summary>
        public string CategoryCode { get; set; }

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
