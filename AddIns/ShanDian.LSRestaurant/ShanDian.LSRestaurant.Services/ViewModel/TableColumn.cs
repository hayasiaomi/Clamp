namespace ShanDian.LSRestaurant.ViewModel
{
    /// <summary>
    /// Sqlite表的字段结构信息
    /// </summary>
    public class TableColumn
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int Cid { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否为空
        /// </summary>
        public int NotNull { get; set; }

        /// <summary>
        /// 字段默认值
        /// </summary>
        public string Dflt_value { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public int Pk { get; set; }
    }
}
