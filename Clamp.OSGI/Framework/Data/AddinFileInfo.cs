using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class AddinFileInfo
    {
        public string File;
        public DateTime LastScan;
        public string AddinId;
        public bool IsRoot;
        public bool ScanError;
        public string Domain;
        public List<string> IgnorePaths;

        public bool IsAddin
        {
            get { return AddinId != null && AddinId.Length != 0; }
        }

        public string FileName { set; get; }
        public System.DateTime Timestamp { set; get; }

        public void AddPathToIgnore(string path)
        {
            if (IgnorePaths == null)
                IgnorePaths = new List<string>();
            IgnorePaths.Add(path);
        }
    }
}
