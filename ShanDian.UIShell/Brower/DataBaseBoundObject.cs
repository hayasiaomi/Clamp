using ShanDian.UIShell.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Brower
{
    public class DataBaseBoundObject
    {
        public void add(string key, string value)
        {
            CDBHelper.Add(key, value);
        }

        public void remove(string key)
        {
            CDBHelper.Remove(key);
        }

        public void modify(string key, string value)
        {
            CDBHelper.Modify(key, value);
        }

        public string get(string key)
        {
            return CDBHelper.Get(key);
        }

        public bool exist(string key)
        {
            return CDBHelper.Exist(key);
        }

    }
}
