using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.DB
{
    public class DataHandler
    {
        public string DBLocation { private set; get; }

        public string DataName { private set; get; }

        public DataHandler(string location, string dataName)
        {
            this.DBLocation = location;
            this.DataName = dataName;

            string dir = Path.GetDirectoryName(this.DBLocation);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
        public void Insert(string key, object value)
        {
            using (var dbLite = new LiteDatabase(this.DBLocation))
            {
                var dbPrinterInfos = dbLite.GetCollection<DictionaryInfo>(this.DataName);

                object dbResult = dbPrinterInfos.FindOne(db => db.KeyName == key);

                if (dbResult == null)
                {
                    dbPrinterInfos.Insert(new DictionaryInfo { KeyName = key, Value = value });
                }
                else
                {
                    DictionaryInfo dictionaryInfo = dbResult as DictionaryInfo;

                    if (dictionaryInfo != null)
                    {
                        dictionaryInfo.Value = value;
                    }

                    dbPrinterInfos.Update(dictionaryInfo);
                }
            }
        }

        public void Update(string key, object value)
        {
            using (var dbLite = new LiteDatabase(DBLocation))
            {
                var dbPrinterInfos = dbLite.GetCollection<DictionaryInfo>(this.DataName);

                object dbResult = dbPrinterInfos.FindOne(db => db.KeyName == key);

                if (dbResult != null)
                {
                    DictionaryInfo dictionaryInfo = dbResult as DictionaryInfo;

                    if (dictionaryInfo != null)
                    {
                        dictionaryInfo.Value = value;
                    }

                    dbPrinterInfos.Update(dictionaryInfo);
                }
            }
        }

        public void Delete(string key)
        {
            using (var dbLite = new LiteDatabase(DBLocation))
            {
                var dbPrinterInfos = dbLite.GetCollection<DictionaryInfo>(this.DataName);

                dbPrinterInfos.Delete(db => db.KeyName == key);
            }
        }

        public DictionaryInfo Get(string key)
        {
            DictionaryInfo dbResult;

            using (var dbLite = new LiteDatabase(this.DBLocation))
            {
                var dbPrinterInfos = dbLite.GetCollection<DictionaryInfo>(this.DataName);

                dbResult = dbPrinterInfos.FindOne(db => db.KeyName == key);
            }

            return dbResult;

        }

        public List<DictionaryInfo> GetAll()
        {
            List<DictionaryInfo> dbResults = null;

            using (var dbLite = new LiteDatabase(this.DBLocation))
            {
                var dbPrinterInfos = dbLite.GetCollection<DictionaryInfo>(this.DataName);

                dbResults = dbPrinterInfos.FindAll().ToList();
            }

            return dbResults;
        }


        public object GetValue(string key)
        {
            DictionaryInfo dbResult = Get(key);

            if (dbResult != null)
                return dbResult.Value;
            return null;
        }

        public bool Exist()
        {
            return File.Exists(this.DBLocation);
        }

        public bool Exist(string key)
        {
            bool exist = false;

            using (var dbLite = new LiteDatabase(DBLocation))
            {
                var dbPrinterInfos = dbLite.GetCollection<DictionaryInfo>(this.DataName);

                exist = dbPrinterInfos.Exists(db => db.KeyName == key);
            }

            return exist;

        }
    }
}
