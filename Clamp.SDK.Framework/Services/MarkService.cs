using LiteDB;
using Clamp.SDK.Framework.Helpers;
using Clamp.SDK.Framework.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Services
{
    public class MarkService : IMarkService
    {

        public void AddMark(string name, string value)
        {
            this.AddMark(name, value, "default");
        }

        public void AddMark(string name, string value, string groupName)
        {
            using (LiteDatabase db = this.GetSettingsLiteDatabase())
            {
                LiteCollection<Mark> settings = db.GetCollection<Mark>(Constants.DatabaseMarkName);

                Mark setting = settings.FindOne(t => t.Name == name && t.GroupName == groupName);

                if (setting != null)
                {
                    setting = new Mark
                    {
                        Value = value,
                    };

                    settings.Update(setting);
                }
                else
                {
                    setting = new Mark
                    {
                        Name = name,
                        Value = value,
                        GroupName = groupName
                    };

                    BsonValue bsonValue = settings.Insert(setting);
                }
            }
        }

        public object GetValueByName(string name)
        {
            return this.GetValueByName(name, "default");
        }

        public object GetValueByName(string name, string groupName)
        {
            using (LiteDatabase db = this.GetSettingsLiteDatabase())
            {
                LiteCollection<Mark> settingCollection = db.GetCollection<Mark>(Constants.DatabaseMarkName);

                Mark marks = settingCollection.FindOne(t => t.Name == name && t.GroupName == groupName);

                if (marks != null)
                {
                    return marks.Value;
                }
            }

            return null;
        }

        public List<Mark> GetMarks(string name)
        {
            List<Mark> marks = new List<Mark>();

            using (LiteDatabase db = this.GetSettingsLiteDatabase())
            {
                LiteCollection<Mark> markCollection = db.GetCollection<Mark>(Constants.DatabaseMarkName);

                List<Mark> mMarks = markCollection.Find(t => t.Name == name).ToList();

                if (mMarks != null && mMarks.Count > 0)
                    marks.AddRange(mMarks);
            }

            return marks;
        }

        public List<object> GetValues(string name)
        {
            List<object> values = new List<object>();

            using (LiteDatabase db = this.GetSettingsLiteDatabase())
            {
                LiteCollection<Mark> markCollection = db.GetCollection<Mark>(Constants.DatabaseMarkName);

                List<Mark> marks = markCollection.Find(t => t.Name == name).ToList();

                if (marks != null && marks.Count > 0)
                {
                    foreach (Mark mark in marks)
                    {
                        values.Add(mark.Value);
                    }
                }
            }

            return values;
        }

        public List<object> GetValuesByGroupName(string groupName)
        {
            List<object> values = new List<object>();

            using (LiteDatabase db = this.GetSettingsLiteDatabase())
            {
                LiteCollection<Mark> markCollection = db.GetCollection<Mark>(Constants.DatabaseMarkName);

                List<Mark> marks = markCollection.Find(t => t.GroupName == groupName).ToList();

                if (marks != null && marks.Count > 0)
                {
                    foreach (Mark mark in marks)
                    {
                        values.Add(mark.Value);
                    }
                }
            }

            return values;
        }

        private LiteDatabase GetSettingsLiteDatabase()
        {
            string dbDir = Path.Combine(SDHelper.GetSDRootPath(), "DB");
            string dbFile = Path.Combine(dbDir, "marks.db");

            if (!Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);

            if (!File.Exists(dbFile))
                File.Create(dbFile).Close();

            return new LiteDatabase(dbFile);
        }

    }
}
