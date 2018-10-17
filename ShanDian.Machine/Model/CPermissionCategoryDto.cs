using System.Collections.Generic;

namespace ShanDian.Machine.Model
{
    /// <summary>
    /// 分类权限配置列表 
    /// </summary>
    public class CPermissionCategoryDto: CPermissionDto
    {
        /// <summary>
        /// 具体权限列
        /// </summary>
        public List<CPermissionCategoryDto> Data { get; set; }= new List<CPermissionCategoryDto>();
    }
}
