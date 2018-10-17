using System.Data.SQLite;
using System.IO;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.LSRestaurant
{
    public static class Global
    {
        static Global()
        {
            string dbFile = Path.Combine(LSRestaurantActivator.AddIn.AddInDirectory, "DB", "Dishes.db");
            ConnectionString = string.Format("Data Source={0};Pooling=true;FailIfMissing=false", dbFile);
            PartName = "Print";
        }

        public static string PartName { get; set; }

        private static string ConnectionString { get; set; }

        public static IRepositoryContext RepositoryContext()
        {
            return Library.GetRepositoryContext(new SQLiteConnection(ConnectionString));
        }
    }
}
