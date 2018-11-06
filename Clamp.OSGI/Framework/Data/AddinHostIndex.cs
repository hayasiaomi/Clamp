using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    class AddinHostIndex
    {
        private Hashtable index = new Hashtable();

        public static AddinHostIndex Read(FileDatabase fileDatabase, string file)
        {
            return (AddinHostIndex)fileDatabase.ReadObject(file);
        }

        public void RegisterAssembly(string assemblyLocation, string addinId, string addinLocation, string domain)
        {
            assemblyLocation = NormalizeFileName(assemblyLocation);
            index[Path.GetFullPath(assemblyLocation)] = addinId + " " + addinLocation + " " + domain;
        }

        string NormalizeFileName(string name)
        {
            return name.ToLower();
        }

        public void Write(FileDatabase fileDatabase, string file)
        {
            fileDatabase.WriteObject(file, this);
        }
    }
}
