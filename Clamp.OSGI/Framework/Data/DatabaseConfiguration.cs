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
        private Dictionary<string, AddinStatus> addinStatus = new Dictionary<string, AddinStatus>();

        internal class AddinStatus
        {
            public AddinStatus(string addinId)
            {
                this.AddinId = addinId;
            }

            public string AddinId;
            public bool Enabled;
            public bool Uninstalled;
            public List<string> Files;
        }

        public bool IsEnabled(string addinId, bool defaultValue)
        {
            var addinName = Bundle.GetIdName(addinId);

            AddinStatus s;

            // If the add-in is globaly disabled, it is disabled no matter what the version specific status is
            if (addinStatus.TryGetValue(addinName, out s))
            {
                if (!s.Enabled)
                    return false;
            }

            if (addinStatus.TryGetValue(addinId, out s))
                return s.Enabled && !IsRegisteredForUninstall(addinId);
            else
                return defaultValue;
        }

        public void SetEnabled(string addinId, bool enabled, bool defaultValue, bool exactVersionMatch)
        {
            if (IsRegisteredForUninstall(addinId))
                return;

            var addinName = exactVersionMatch ? addinId : Bundle.GetIdName(addinId);

            AddinStatus s;
            addinStatus.TryGetValue(addinName, out s);

            if (s == null)
                s = addinStatus[addinName] = new AddinStatus(addinName);
            s.Enabled = enabled;

            // If enabling a specific version of an add-in, make sure the add-in is enabled as a whole
            if (enabled && exactVersionMatch)
                SetEnabled(addinId, true, defaultValue, false);
        }

        public void RegisterForUninstall(string addinId, IEnumerable<string> files)
        {
            AddinStatus s;
            if (!addinStatus.TryGetValue(addinId, out s))
                s = addinStatus[addinId] = new AddinStatus(addinId);

            s.Enabled = false;
            s.Uninstalled = true;
            s.Files = new List<string>(files);
        }

        public void UnregisterForUninstall(string addinId)
        {
            addinStatus.Remove(addinId);
        }

        public bool IsRegisteredForUninstall(string addinId)
        {
            AddinStatus s;
            if (addinStatus.TryGetValue(addinId, out s))
                return s.Uninstalled;
            else
                return false;
        }

        public bool HasPendingUninstalls
        {
            get { return addinStatus.Values.Where(s => s.Uninstalled).Any(); }
        }

        public AddinStatus[] GetPendingUninstalls()
        {
            return addinStatus.Values.Where(s => s.Uninstalled).ToArray();
        }

        public static DatabaseConfiguration Read(string file)
        {
            var config = ReadInternal(file);
            // Try to read application level config to support disabling add-ins by default.
            var appConfig = ReadAppConfig();

            if (appConfig == null)
                return config;

            // Overwrite app config values with user config values
            foreach (var entry in config.addinStatus)
                appConfig.addinStatus[entry.Key] = entry.Value;

            return appConfig;
        }

        public static DatabaseConfiguration ReadAppConfig()
        {
            var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            var appAddinsConfigFilePath = Path.Combine(assemblyDirectory, "addins-config.xml");

            if (!File.Exists(appAddinsConfigFilePath))
                return new DatabaseConfiguration();

            return ReadInternal(appAddinsConfigFilePath);
        }

        static DatabaseConfiguration ReadInternal(string file)
        {
            DatabaseConfiguration config = new DatabaseConfiguration();
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            XmlElement disabledElem = (XmlElement)doc.DocumentElement.SelectSingleNode("DisabledAddins");
            if (disabledElem != null)
            {
                // For back compatibility
                foreach (XmlElement elem in disabledElem.SelectNodes("Addin"))
                    config.SetEnabled(elem.InnerText, false, true, false);
                return config;
            }

            XmlElement statusElem = (XmlElement)doc.DocumentElement.SelectSingleNode("AddinStatus");
            if (statusElem != null)
            {
                foreach (XmlElement elem in statusElem.SelectNodes("Addin"))
                {
                    AddinStatus status = new AddinStatus(elem.GetAttribute("id"));
                    string senabled = elem.GetAttribute("enabled");
                    status.Enabled = senabled.Length == 0 || senabled == "True";
                    status.Uninstalled = elem.GetAttribute("uninstalled") == "True";
                    config.addinStatus[status.AddinId] = status;
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

                tw.WriteStartElement("AddinStatus");
                foreach (AddinStatus e in addinStatus.Values)
                {
                    tw.WriteStartElement("Addin");
                    tw.WriteAttributeString("id", e.AddinId);
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
                tw.WriteEndElement(); // AddinStatus
                tw.WriteEndElement(); // Configuration
            }
        }
    }
}
