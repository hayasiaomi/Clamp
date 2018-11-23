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
        private BundleInfo bundleInfo;
        private string sourceFile;
        private WeakReference desc;
        private BundleDatabase database;
        private bool? isLatestVersion;
        private bool? isUserBundle;
        private string id;
        private string domain;
        private ClampBundle clampBundle;


        internal Bundle()
        {
            this.bundleInfo = null;
            this.sourceFile = null;
            this.desc = null;
            this.domain = null;
            this.clampBundle = null;
            this.database = null;
        }

        internal Bundle(ClampBundle clampBundle, BundleDatabase database, string domain, string id)
        {
            this.clampBundle = clampBundle;
            this.database = database;
            this.id = id;
            this.domain = domain;

            LoadBundleInfo();
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
            get { return this.BundleInfo.Namespace; }
        }

        /// <summary>
        /// Identifier of the add-in (without namespace)
        /// </summary>
        public string LocalId
        {
            get { return this.BundleInfo.LocalId; }
        }

        /// <summary>
        /// Version of the add-in
        /// </summary>
        public string Version
        {
            get { return this.BundleInfo.Version; }
        }

        /// <summary>
        /// Display name of the add-in
        /// </summary>
        public string Name
        {
            get { return this.BundleInfo.Name; }
        }

        /// <summary>
        /// Custom properties specified in the add-in header
        /// </summary>
        public BundlePropertyCollection Properties
        {
            get { return this.BundleInfo.Properties; }
        }


        public bool Enabled
        {
            get
            {
                if (!IsLatestVersion)
                    return false;
                return BundleInfo.IsRoot ? true : database.IsBundleEnabled(Description.Domain, BundleInfo.Id, true);
            }
            set
            {
                if (value)
                    database.EnableBundle(Description.Domain, BundleInfo.Id, true);
                else
                    database.DisableBundle(Description.Domain, BundleInfo.Id);
            }
        }

        /// <summary>
        /// 获得Bundle的细详信息
        /// </summary>
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

                var mBundleFile = database.GetDescriptionPath(domain, id);

                BundleDescription m;

                database.ReadBundleDescription(mBundleFile, out m);

                if (m == null)
                {
                    try
                    {
                        //Bundle的详细是一个空的，但是数据文件却在，所以是脏数据。要删除掉
                        if (File.Exists(mBundleFile))
                        {
                            File.Delete(mBundleFile);
                        }
                    }
                    catch
                    {
                        // Ignore
                    }

                    throw new InvalidOperationException("不能加载Bundle的细详信息");
                }

                if (bundleInfo == null)
                {
                    bundleInfo = BundleInfo.ReadFromDescription(m);
                    sourceFile = m.BundleFile;
                }

                SetIsUserBundle(m);

                if (!isUserBundle.Value)
                    m.Flags |= BundleFlags.CantUninstall;

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


        internal bool IsLatestVersion
        {
            get
            {
                if (isLatestVersion == null)
                {
                    string id, version;
                    Bundle.GetIdParts(BundleInfo.Id, out id, out version);
                    var addins = database.GetInstalledBundles(null, BundleSearchFlagsInternal.IncludeAll | BundleSearchFlagsInternal.LatestVersionsOnly);
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
            get { return Path.Combine(database.BundlePrivateDataPath, Path.GetFileNameWithoutExtension(Description.FileName)); }
        }

        internal BundleInfo BundleInfo
        {
            get
            {
                if (bundleInfo == null)
                {
                    try
                    {
                        bundleInfo = BundleInfo.ReadFromDescription(Description);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Could not read add-in file: " + database.GetDescriptionPath(domain, id), ex);
                    }
                }
                return bundleInfo;
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
            return BundleInfo.SupportsVersion(version);
        }


        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
        }


        #endregion

        #region private method

        /// <summary>
        /// 加载Bundle的相关信息
        /// </summary>
        private void LoadBundleInfo()
        {
            if (bundleInfo == null)
            {
                try
                {
                    BundleDescription m = Description;
                    sourceFile = m.BundleFile;
                    bundleInfo = BundleInfo.ReadFromDescription(m);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Could not read add-in file: " + database.GetDescriptionPath(domain, id), ex);
                }
            }
        }

        /// <summary>
        /// 设置他是否为用户组件
        /// </summary>
        /// <param name="adesc"></param>
        private void SetIsUserBundle(BundleDescription adesc)
        {
            string installPath = database.Registry.DefaultBundlesFolder;

            if (installPath[installPath.Length - 1] != Path.DirectorySeparatorChar)
                installPath += Path.DirectorySeparatorChar;

            isUserBundle = adesc != null && Path.GetFullPath(adesc.BundleFile).StartsWith(installPath);
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
