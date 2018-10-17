using System;
using Dapper.Contrib.Extensions;

namespace ShanDian.LSRestaurant.Model.Dishes
{
    [Table("tb_dbhistory")]
    public class DbHistory
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 数据库版本（应与组件版本相同）
        /// </summary>
        public string VersionCode { get; set; }

        /// <summary>
        /// 版本升级时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
