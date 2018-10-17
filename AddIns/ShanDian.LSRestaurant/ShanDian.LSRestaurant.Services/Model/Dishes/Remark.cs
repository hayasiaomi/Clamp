using System;
using Dapper.Contrib.Extensions;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    /// <summary>
    /// 菜品备注
    /// </summary>
    [Table("tb_remarks")]
    public class Remark
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 备注内容
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// 修改时间（按使用时间提取最近10条显示）
        /// </summary>
        public DateTime ModificationTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public string Modifitor { get; set; }
    }
}
