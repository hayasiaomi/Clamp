using Clamp.OSGI.Framework.Data;
using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework
{
    /// <summary>
    /// 插件类
    /// </summary>
    public class Bundle : IBundle
    {
        private BundleInfo addin;
        private string sourceFile;
        private WeakReference desc;
        private BundleDatabase database;
        private bool? isLatestVersion;
        private bool? isUserAddin;
        private string id;
        private string domain;
        private BundleRegistry registry;
        private ClampBundle clampBundle;

        internal Bundle()
        {
            this.addin = null;
            this.sourceFile = null;
            this.desc = null;
            this.domain = null;
            this.clampBundle = null;
            this.database = null;
            this.registry = null;
        }

        internal Bundle(ClampBundle clampBundle, BundleDatabase database, string domain, string id)
        {
            this.clampBundle = clampBundle;
            this.database = database;
            this.id = id;
            this.domain = domain;
            LoadAddinInfo();
        }

        private void LoadAddinInfo()
        {
            if (addin == null)
            {
                try
                {
                    BundleDescription m = Description;
                    sourceFile = m.AddinFile;
                    addin = BundleInfo.ReadFromDescription(m);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Could not read add-in file: " + database.GetDescriptionPath(domain, id), ex);
                }
            }
        }
        #region public Property
        /// <summary>
        /// Full identifier of the add-in, including namespace and version.
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// Namespace of the add-in.
        /// </summary>
        public string Namespace
        {
            get { return this.AddinInfo.Namespace; }
        }

        /// <summary>
        /// Identifier of the add-in (without namespace)
        /// </summary>
        public string LocalId
        {
            get { return this.AddinInfo.LocalId; }
        }

        /// <summary>
        /// Version of the add-in
        /// </summary>
        public string Version
        {
            get { return this.AddinInfo.Version; }
        }

        /// <summary>
        /// Display name of the add-in
        /// </summary>
        public string Name
        {
            get { return this.AddinInfo.Name; }
        }

        /// <summary>
        /// Custom properties specified in the add-in header
        /// </summary>
        public AddinPropertyCollection Properties
        {
            get { return this.AddinInfo.Properties; }
        }


        public bool Enabled
        {
            get
            {
                if (!IsLatestVersion)
                    return false;
                return AddinInfo.IsRoot ? true : database.IsAddinEnabled(Description.Domain, AddinInfo.Id, true);
            }
            set
            {
                if (value)
                    database.EnableAddin(Description.Domain, AddinInfo.Id, true);
                else
                    database.DisableAddin(Description.Domain, AddinInfo.Id);
            }
        }

        public BundleDescription Description
        {
            get
            {
                if (desc != null)
                {
                    BundleDescription d = desc.Target as BundleDescription;
                    if (d != null)
                        return d;
                }

                var configFile = database.GetDescriptionPath(domain, id);

                BundleDescription m;

                database.ReadAddinDescription(configFile, out m);

                if (m == null)
                {
                    try
                    {
                        if (File.Exists(configFile))
                        {
                            // The file is corrupted. Remove it.
                            File.Delete(configFile);
                        }
                    }
                    catch
                    {
                        // Ignore
                    }
                    throw new InvalidOperationException("Could not read add-in description");
                }
                if (addin == null)
                {
                    addin = BundleInfo.ReadFromDescription(m);
                    sourceFile = m.AddinFile;
                }
                SetIsUserAddin(m);
                if (!isUserAddin.Value)
                    m.Flags |= AddinFlags.CantUninstall;
                desc = new WeakReference(m);
                return m;
            }
        }
        #endregion
        #region internal Property
        internal ClampBundle ClampBundle
        {
            get { return this.clampBundle; }
        }

        internal BundleRegistry Registry
        {
            get
            {
                //CheckInitialized();
                return registry;
            }
        }

        internal bool IsLatestVersion
        {
            get
            {
                if (isLatestVersion == null)
                {
                    string id, version;
                    Bundle.GetIdParts(AddinInfo.Id, out id, out version);
                    var addins = database.GetInstalledAddins(null, AddinSearchFlagsInternal.IncludeAll | AddinSearchFlagsInternal.LatestVersionsOnly);
                    isLatestVersion = addins.Any(a => Bundle.GetIdName(a.Id) == id && a.Version == version);
                }
                return isLatestVersion.Value;
            }
            set
            {
                isLatestVersion = value;
            }
        }
        internal string PrivateDataPath
        {
            get { return Path.Combine(database.AddinPrivateDataPath, Path.GetFileNameWithoutExtension(Description.FileName)); }
        }

        internal BundleInfo AddinInfo
        {
            get
            {
                if (addin == null)
                {
                    try
                    {
                        addin = BundleInfo.ReadFromDescription(Description);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Could not read add-in file: " + database.GetDescriptionPath(domain, id), ex);
                    }
                }
                return addin;
            }
        }
        #endregion
        #region public method

        /// <summary>
        /// Checks version compatibility.
        /// </summary>
        /// <param name="version">
        /// An add-in version.
        /// </param>
        /// <returns>
        /// True if the provided version is compatible with this add-in.
        /// </returns>
        /// <remarks>
        /// This method checks the CompatVersion property to know if the provided version is compatible with the version of this add-in.
        /// </remarks>
        public bool SupportsVersion(string version)
        {
            return AddinInfo.SupportsVersion(version);
        }


        public virtual void Start()
        {
            throw new NotImplementedException();
        }

        public virtual void Stop()
        {
            throw new NotImplementedException();
        }


        #endregion

        #region private method
        private void SetIsUserAddin(BundleDescription adesc)
        {
            string installPath = database.Registry.DefaultAddinsFolder;

            if (installPath[installPath.Length - 1] != Path.DirectorySeparatorChar)
                installPath += Path.DirectorySeparatorChar;
            isUserAddin = adesc != null && Path.GetFullPath(adesc.AddinFile).StartsWith(installPath);
        }
        #endregion

        #region public static method

        public static string GetFullId(string ns, string id, string version)
        {
            string res;
            if (id.StartsWith("::"))
                res = id.Substring(2);
            else if (ns != null && ns.Length > 0)
                res = ns + "." + id;
            else
                res = id;

            if (version != null && version.Length > 0)
                return res + "," + version;
            else
                return res;
        }

        public static string GetIdName(string addinId)
        {
            int i = addinId.IndexOf(',');
            if (i != -1)
                return addinId.Substring(0, i);
            else
                return addinId;
        }

        public static string GetIdVersion(string addinId)
        {
            int i = addinId.IndexOf(',');
            if (i != -1)
                return addinId.Substring(i + 1).Trim();
            else
                return string.Empty;
        }

        public static int CompareVersions(string v1, string v2)
        {
            string[] a1 = v1.Split('.');
            string[] a2 = v2.Split('.');

            for (int n = 0; n < a1.Length; n++)
            {
                if (n >= a2.Length)
                    return -1;
                if (a1[n].Length == 0)
                {
                    if (a2[n].Length != 0)
                        return 1;
                    continue;
                }
                try
                {
                    int n1 = int.Parse(a1[n]);
                    int n2 = int.Parse(a2[n]);
                    if (n1 < n2)
                        return 1;
                    else if (n1 > n2)
                        return -1;
                }
                catch
                {
                    return 1;
                }
            }
            if (a2.Length > a1.Length)
                return 1;
            return 0;
        }
        public static void GetIdParts(string addinId, out string name, out string version)
        {
            int i = addinId.IndexOf(',');
            if (i != -1)
            {
                name = addinId.Substring(0, i);
                version = addinId.Substring(i + 1).Trim();
            }
            else
            {
                name = addinId;
                version = string.Empty;
            }
        }
        #endregion


    }
}
