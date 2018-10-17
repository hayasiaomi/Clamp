using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.DB
{
    public static class Library
    {
        public static IRepositoryContext GetRepositoryContext(IDbConnection dbConnection)
        {
            if (!string.IsNullOrWhiteSpace(dbConnection.ConnectionString))
            {
                string[] dbSettings = dbConnection.ConnectionString.Split(';');

                if (dbSettings != null && dbSettings.Length > 0)
                {
                    foreach (string dbSetting in dbSettings)
                    {
                        string dbSettingTrim = dbSetting.Trim();

                        if (dbSettingTrim.StartsWith("DATA", StringComparison.CurrentCultureIgnoreCase) && dbSettingTrim.IndexOf("SOURCE", StringComparison.CurrentCultureIgnoreCase) > 0)
                        {
                            int eqIndex = dbSettingTrim.IndexOf("=");

                            string dbFile = dbSettingTrim.Substring(eqIndex + 1);
                            string dbDir = Path.GetDirectoryName(dbFile);

                            if (!Directory.Exists(dbDir))
                                Directory.CreateDirectory(dbDir);

                            if (!string.IsNullOrWhiteSpace(dbFile) && !File.Exists(dbFile))
                            {
                                File.Create(dbFile).Close();
                            }
                        }
                    }
                }
            }

            return new RepositoryContext(dbConnection);
        }
    }
}
