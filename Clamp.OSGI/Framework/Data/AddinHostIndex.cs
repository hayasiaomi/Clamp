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


        public void RemoveHostData(string addinId, string addinLocation)
        {
            string loc = addinId + " " + Path.GetFullPath(addinLocation) + " ";
            ArrayList todelete = new ArrayList();
            foreach (DictionaryEntry e in index)
            {
                if (((string)e.Value).StartsWith(loc))
                    todelete.Add(e.Key);
            }
            foreach (string s in todelete)
                index.Remove(s);
        }
        string NormalizeFileName(string name)
        {
            return name.ToLower();
        }

        public void Write(FileDatabase fileDatabase, string file)
        {
            fileDatabase.WriteObject(file, this);
        }

        public bool GetAddinForAssembly(string assemblyLocation, out string addinId, out string addinLocation, out string domain)
        {
            assemblyLocation = NormalizeFileName(assemblyLocation);
            string s = index[Path.GetFullPath(assemblyLocation)] as string;
            if (s == null)
            {
                addinId = null;
                addinLocation = null;
                domain = null;
                return false;
            }
            else
            {
                int i = s.IndexOf(' ');
                int j = s.LastIndexOf(' ');
                addinId = s.Substring(0, i);
                addinLocation = s.Substring(i + 1, j - i - 1);
                domain = s.Substring(j + 1);
                return true;
            }
        }
    }
}
