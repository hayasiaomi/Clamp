namespace ShanDian.LSRestaurant.ViewModel
{
    /// <summary>
    /// sqlite_master 结构
    /// </summary>
    public class SqliteMaster
    {
        /// <summary>
        /// 类型：表 [type]='table'
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 名称（表名）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string Tbl_Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RootPage { get; set; }

        /// <summary>
        /// SQL语句（CREATE TABLE ）
        /// </summary>
        public string Sql { get; set; }
    }
}
