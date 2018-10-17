using ShanDian.AddIns.Print;
using ShanDian.SDK.Framework.DB;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDian.AddIns.Print
{
    public static class Global
    {
        static Global()
        {
            string dbFile = Path.Combine(PrintActivator.AddIn.AddInDirectory, "DB", "Print.db");
            //dbFile = @"E:\Git\20180830\ShanDian.Src\Bin\DB\Print.db";
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
