using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class AddinScanResult : MarshalByRefObject, IAssemblyLocator
    {
        private ArrayList filesToScan = new ArrayList();
        private Hashtable visitedFolders = new Hashtable();
        private Hashtable assemblyLocations = new Hashtable();
        private Hashtable assemblyLocationsByFullName = new Hashtable();
        private Hashtable filesToIgnore;
        private bool regenerateRelationData;

        public bool RegenerateAllData { set; get; }

        public bool CheckOnly { set; get; }

        public string Domain { set; get; }

        public bool ChangesFound { set; get; }

        public bool LocateAssembliesOnly { set; get; }


        public bool RegenerateRelationData
        {
            get { return this.regenerateRelationData; }
            set
            {
                this.regenerateRelationData = value;
                if (value)
                    this.ChangesFound = true;
            }
        }

        internal ArrayList ModifiedFolderInfos { set; get; }

        internal List<string> AddinsToUpdateRelations { set; get; }

        internal List<string> RemovedAddins { set; get; }

        internal ArrayList FilesToScan { get { return this.filesToScan; } }

        internal List<string> AddinsToUpdate { set; get; }

        internal AddinHostIndex HostIndex { set; get; }

        public void RegisterModifiedFolderInfo(BundleScanFolderInfo folderInfo)
        {
            if (!this.ModifiedFolderInfos.Contains(folderInfo))
                this.ModifiedFolderInfos.Add(folderInfo);
        }

        public void AddRemovedAddin(string addinId)
        {
            if (!this.RemovedAddins.Contains(addinId))
                this.RemovedAddins.Add(addinId);
        }

        public void AddAddinToUpdateRelations(string addinId)
        {
            if (!this.AddinsToUpdateRelations.Contains(addinId))
                this.AddinsToUpdateRelations.Add(addinId);
        }

        public bool VisitFolder(string folder)
        {
            if (visitedFolders.Contains(folder) || IgnorePath(folder))
                return false;
            else
            {
                visitedFolders.Add(folder, folder);
                return true;
            }
        }

        public void AddAddinToUpdate(string addinId)
        {
            if (!AddinsToUpdate.Contains(addinId))
                AddinsToUpdate.Add(addinId);
        }

        public bool IgnorePath(string file)
        {
            if (filesToIgnore == null)
                return false;
            string root = Path.GetPathRoot(file);
            while (root != file)
            {
                if (filesToIgnore.Contains(file))
                    return true;
                file = Path.GetDirectoryName(file);
            }
            return false;
        }

        public void AddFileToScan(string file, BundleScanFolderInfo folderInfo)
        {
            FileToScan di = new FileToScan();
            di.File = file;
            di.AddinScanFolderInfo = folderInfo;
            FilesToScan.Add(di);
            RegisterModifiedFolderInfo(folderInfo);
        }


        public void AddPathsToIgnore(IEnumerable paths)
        {
            foreach (string p in paths)
                AddPathToIgnore(p);
        }

        public void AddPathToIgnore(string path)
        {
            if (filesToIgnore == null)
                filesToIgnore = new Hashtable();
            filesToIgnore[path] = path;
        }

        public void AddAssemblyLocation(string file)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            ArrayList list = assemblyLocations[name] as ArrayList;
            if (list == null)
            {
                list = new ArrayList();
                assemblyLocations[name] = list;
            }
            list.Add(file);
        }

        public string GetAssemblyLocation(string fullName)
        {
            string loc = assemblyLocationsByFullName[fullName] as String;

            if (loc != null)
                return loc;

            int i = fullName.IndexOf(',');
            string name = fullName.Substring(0, i);

            if (name == "Clamp.OSGI")
                return GetType().Assembly.Location;

            ArrayList list = assemblyLocations[name] as ArrayList;
            if (list == null)
                return null;

            string lastAsm = null;
            foreach (string file in list.ToArray())
            {
                try
                {
                    list.Remove(file);
                    AssemblyName aname = AssemblyName.GetAssemblyName(file);
                    lastAsm = file;
                    assemblyLocationsByFullName[aname.FullName] = file;
                    if (aname.FullName == fullName)
                        return file;
                }
                catch
                {
                    // Could not get the assembly name. The file either doesn't exist or it is not a valid assembly.
                    // In this case, just ignore it.
                }
            }

            if (lastAsm != null)
            {
                // If an exact version is not found, just take any of them
                return lastAsm;
            }
            return null;
        }


    }

    class FileToScan
    {
        public string File;
        public BundleScanFolderInfo AddinScanFolderInfo;
    }
}
