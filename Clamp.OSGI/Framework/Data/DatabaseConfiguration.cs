using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.OSGI.Framework.Data
{
    internal class DatabaseConfiguration
    {
        private Dictionary<string, BundleStatus> bundleStatus = new Dictionary<string, BundleStatus>();

        internal class BundleStatus
        {
            public BundleStatus(string bundleId)
            {
                this.BundleId = bundleId;
            }

            public string BundleId;
            public bool Enabled;
            public bool Uninstalled;
            public List<string> Files;
        }

        public bool IsEnabled(string addinId, bool defaultValue)
        {
            var addinName = Bundle.GetIdName(addinId);

            BundleStatus s;

            // If the add-in is globaly disabled, it is disabled no matter what the version specific status is
            if (bundleStatus.TryGetValue(addinName, out s))
            {
                if (!s.Enabled)
                    return false;
            }

            if (bundleStatus.TryGetValue(addinId, out s))
                return s.Enabled && !IsRegisteredForUninstall(addinId);
            else
                return defaultValue;
        }

        public void SetEnabled(string addinId, bool enabled, bool defaultValue, bool exactVersionMatch)
        {
            if (IsRegisteredForUninstall(addinId))
                return;

            var bundleName = exactVersionMatch ? addinId : Bundle.GetIdName(addinId);

            BundleStatus s;
            bundleStatus.TryGetValue(bundleName, out s);

            if (s == null)
                s = bundleStatus[bundleName] = new BundleStatus(bundleName);
            s.Enabled = enabled;

            // If enabling a specific version of an add-in, make sure the add-in is enabled as a whole
            if (enabled && exactVersionMatch)
                SetEnabled(addinId, true, defaultValue, false);
        }

        public void RegisterForUninstall(string addinId, IEnumerable<string> files)
        {
            BundleStatus s;
            if (!bundleStatus.TryGetValue(addinId, out s))
                s = bundleStatus[addinId] = new BundleStatus(addinId);

            s.Enabled = false;
            s.Uninstalled = true;
            s.Files = new List<string>(files);
        }

        public void UnregisterForUninstall(string addinId)
        {
            bundleStatus.Remove(addinId);
        }

        public bool IsRegisteredForUninstall(string addinId)
        {
            BundleStatus s;
            if (bundleStatus.TryGetValue(addinId, out s))
                return s.Uninstalled;
            else
                return false;
        }

        public bool HasPendingUninstalls
        {
            get { return bundleStatus.Values.Where(s => s.Uninstalled).Any(); }
        }

        public BundleStatus[] GetPendingUninstalls()
        {
            return bundleStatus.Values.Where(s => s.Uninstalled).ToArray();
        }

        public static DatabaseConfiguration Read(string file)
        {
            var config = ReadInternal(file);
            // Try to read application level config to support disabling add-ins by default.
            var appConfig = ReadAppConfig();

            if (appConfig == null)
                return config;

            // Overwrite app config values with user config values
            foreach (var entry in config.bundleStatus)
                appConfig.bundleStatus[entry.Key] = entry.Value;

            return appConfig;
        }

        public static DatabaseConfiguration ReadAppConfig()
        {
            var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            var appBundlesConfigFilePath = Path.Combine(assemblyDirectory, "addins-config.xml");

            if (!File.Exists(appBundlesConfigFilePath))
                return new DatabaseConfiguration();

            return ReadInternal(appBundlesConfigFilePath);
        }

        static DatabaseConfiguration ReadInternal(string file)
        {
            DatabaseConfiguration config = new DatabaseConfiguration();
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlElement disabledElem = (XmlElement)doc.DocumentElement.SelectSingleNode("DisabledBundles");
            if (disabledElem != null)
            {
                // For back compatibility
                foreach (XmlElement elem in disabledElem.SelectNodes("Bundle"))
                    config.SetEnabled(elem.InnerText, false, true, false);
                return config;
            }

            XmlElement statusElem = (XmlElement)doc.DocumentElement.SelectSingleNode("BundleStatus");
            if (statusElem != null)
            {
                foreach (XmlElement elem in statusElem.SelectNodes("Bundle"))
                {
                    BundleStatus status = new BundleStatus(elem.GetAttribute("id"));
                    string senabled = elem.GetAttribute("enabled");
                    status.Enabled = senabled.Length == 0 || senabled == "True";
                    status.Uninstalled = elem.GetAttribute("uninstalled") == "True";
                    config.bundleStatus[status.BundleId] = status;
                    foreach (XmlElement fileElem in elem.SelectNodes("File"))
                    {
                        if (status.Files == null)
                            status.Files = new List<string>();
                        status.Files.Add(fileElem.InnerText);
                    }
                }
            }
            return config;
        }

        public void Write(string file)
        {
            StreamWriter s = new StreamWriter(file);
            using (s)
            {
                XmlTextWriter tw = new XmlTextWriter(s);
                tw.Formatting = Formatting.Indented;
                tw.WriteStartElement("Configuration");

                tw.WriteStartElement("BundleStatus");
                foreach (BundleStatus e in bundleStatus.Values)
                {
                    tw.WriteStartElement("Bundle");
                    tw.WriteAttributeString("id", e.BundleId);
                    tw.WriteAttributeString("enabled", e.Enabled.ToString());
                    if (e.Uninstalled)
                        tw.WriteAttributeString("uninstalled", "True");
                    if (e.Files != null && e.Files.Count > 0)
                    {
                        foreach (var f in e.Files)
                            tw.WriteElementString("File", f);
                    }
                    tw.WriteEndElement();
                }
                tw.WriteEndElement(); // BundleStatus
                tw.WriteEndElement(); // Configuration
            }
        }
    }
}
