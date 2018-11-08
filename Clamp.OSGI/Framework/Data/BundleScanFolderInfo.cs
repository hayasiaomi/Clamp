using Clamp.OSGI.Framework.Data.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class BundleScanFolderInfo : IBinaryXmlElement
    {
        private static BinaryXmlTypeMap typeMap = new BinaryXmlTypeMap(typeof(BundleScanFolderInfo), typeof(BundleFileInfo));

        private Hashtable files = new Hashtable();
        private string folder;
        private string fileName;
        private string domain;
        private bool sharedFolder = true;

        public string Folder
        {
            get { return folder; }
        }

        public bool SharedFolder
        {
            get
            {
                return sharedFolder;
            }
            set
            {
                sharedFolder = value;
            }
        }
        public string FileName
        {
            get { return fileName; }
        }

        public string Domain
        {
            get
            {
                if (sharedFolder)
                    return BundleDatabase.GlobalDomain;
                else
                    return domain;
            }
            set
            {
                domain = value;
                sharedFolder = true;
            }
        }

        public string RootsDomain
        {
            get
            {
                return domain;
            }
            set
            {
                domain = value;
            }
        }


        public BundleScanFolderInfo(string folder)
        {
            this.folder = folder;
        }

        internal BundleScanFolderInfo()
        {

        }
        #region public method

        public void Write(FileDatabase filedb, string basePath)
        {
            filedb.WriteSharedObject(basePath, GetDomain(folder), ".data", Path.GetFullPath(folder), fileName, typeMap, this);
        }

        public BundleFileInfo SetLastScanTime(string file, string addinId, bool isRoot, DateTime time, bool scanError)
        {
            BundleFileInfo info = (BundleFileInfo)files[file];
            if (info == null)
            {
                info = new BundleFileInfo();
                info.File = file;
                files[file] = info;
            }
            info.LastScan = time;
            info.BundleId = addinId;
            info.IsRoot = isRoot;
            info.ScanError = scanError;
            if (addinId != null)
                info.Domain = GetDomain(isRoot);
            else
                info.Domain = null;
            return info;
        }

        public BundleFileInfo GetBundleFileInfo(string file)
        {
            return (BundleFileInfo)files[file];
        }

        public ArrayList GetMissingBundles(BundleFileSystemExtension fs)
        {
            ArrayList missing = new ArrayList();

            if (!fs.DirectoryExists(folder))
            {
                // All deleted
                foreach (BundleFileInfo info in files.Values)
                {
                    if (info.IsBundle)
                        missing.Add(info);
                }
                files.Clear();
                return missing;
            }
            ArrayList toDelete = new ArrayList();

            foreach (BundleFileInfo info in files.Values)
            {
                if (!fs.FileExists(info.File))
                {
                    if (info.IsBundle)
                        missing.Add(info);

                    toDelete.Add(info.File);
                }
                else if (info.IsBundle && info.Domain != GetDomain(info.IsRoot))
                {
                    missing.Add(info);
                }
            }

            foreach (string file in toDelete)
                files.Remove(file);

            return missing;
        }
        #endregion

        #region public static method


        public static BundleScanFolderInfo Read(FileDatabase filedb, string file)
        {
            BundleScanFolderInfo finfo = (BundleScanFolderInfo)filedb.ReadSharedObject(file, typeMap);

            if (finfo != null)
                finfo.fileName = file;
            return finfo;
        }


        public static BundleScanFolderInfo Read(FileDatabase filedb, string basePath, string folderPath)
        {
            string fileName;
            BundleScanFolderInfo finfo = (BundleScanFolderInfo)filedb.ReadSharedObject(basePath, GetDomain(folderPath), ".data", Path.GetFullPath(folderPath), typeMap, out fileName);
            if (finfo != null)
                finfo.fileName = fileName;
            return finfo;
        }

        public static string GetDomain(string path)
        {
            path = Path.GetFullPath(path);
            string s = path.Replace(Path.DirectorySeparatorChar, '_');
            s = s.Replace(Path.AltDirectorySeparatorChar, '_');
            s = s.Replace(Path.VolumeSeparatorChar, '_');
            s = s.Trim('_');
            return s;
        }

        public string GetDomain(bool isRoot)
        {
            if (isRoot)
                return RootsDomain;
            else
                return Domain;
        }
        #endregion


        void IBinaryXmlElement.Write(BinaryXmlWriter writer)
        {
            if (files.Count == 0)
            {
                domain = null;
                sharedFolder = true;
            }
            writer.WriteValue("folder", folder);
            writer.WriteValue("files", files);
            writer.WriteValue("domain", domain);
            writer.WriteValue("sharedFolder", sharedFolder);
        }

        void IBinaryXmlElement.Read(BinaryXmlReader reader)
        {
            folder = reader.ReadStringValue("folder");
            reader.ReadValue("files", files);
            domain = reader.ReadStringValue("domain");
            sharedFolder = reader.ReadBooleanValue("sharedFolder");
        }
    }

    class BundleFileInfo : IBinaryXmlElement
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

        public void AddPathToIgnore(string path)
        {
            if (IgnorePaths == null)
                IgnorePaths = new StringCollection();
            IgnorePaths.Add(path);
        }

        void IBinaryXmlElement.Write(BinaryXmlWriter writer)
        {
            writer.WriteValue("File", File);
            writer.WriteValue("LastScan", LastScan);
            writer.WriteValue("BundleId", BundleId);
            writer.WriteValue("IsRoot", IsRoot);
            writer.WriteValue("ScanError", ScanError);
            writer.WriteValue("Domain", Domain);
            if (IgnorePaths != null && IgnorePaths.Count > 0)
                writer.WriteValue("IgnorePaths", IgnorePaths);
        }

        void IBinaryXmlElement.Read(BinaryXmlReader reader)
        {
            File = reader.ReadStringValue("File");
            LastScan = reader.ReadDateTimeValue("LastScan");
            BundleId = reader.ReadStringValue("BundleId");
            IsRoot = reader.ReadBooleanValue("IsRoot");
            ScanError = reader.ReadBooleanValue("ScanError");
            Domain = reader.ReadStringValue("Domain");
            IgnorePaths = (StringCollection)reader.ReadValue("IgnorePaths", new StringCollection());
        }
    }
}
