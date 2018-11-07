using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class BundleFileInfo
    {
        public string File;
        public DateTime LastScan;
        public string BundleId;
        public bool IsRoot;
        public bool ScanError;
        public string Domain;
        public StringCollection IgnorePaths;

        public bool IsBundle
        {
            get { return BundleId != null && BundleId.Length != 0; }
        }

        public string FileName { set; get; }
        public System.DateTime Timestamp { set; get; }

        public void AddPathToIgnore(string path)
        {
            if (IgnorePaths == null)
                IgnorePaths = new StringCollection();
            IgnorePaths.Add(path);
        }
    }
}
