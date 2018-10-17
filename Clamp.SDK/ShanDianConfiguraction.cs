using Clamp.Common.Initial;
using Clamp.SDK.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.SDK
{
    class ShanDianConfiguraction
    {
        public string ConfigurationString { private set; get; }

        public string UpdateProcessPath { private set; get; }
        public string UpdateCheckURL { private set; get; }
        public string UpdateDownloadLocation { private set; get; }

        public int UpdateInterval { private set; get; }

        public string SDShellPathName { set; get; }

        public ShanDianConfiguraction() : this(Path.Combine(SDHelper.GetSDRootPath(), "shandian.ini"))
        {

        }

        public ShanDianConfiguraction(string configurationString)
        {
            this.ConfigurationString = configurationString;
        }

        public void Initialize()
        {
            FileInfo fileInfo = new FileInfo(this.ConfigurationString);

            InitialFile initials = InitialFile.LoadFromFile(this.ConfigurationString);

            if (!fileInfo.Exists || fileInfo.Length <= 0)
            {
                this.UpdateCheckURL = "";
                this.UpdateDownloadLocation = "Upgrades";
                this.UpdateProcessPath = "";
                this.UpdateInterval = 5;
                this.SDShellPathName = "Desktops";

                initials["UpdateProcessPath"].StringValue = this.UpdateProcessPath;
                initials["UpdateDownloadLocation"].StringValue = this.UpdateDownloadLocation;
                initials["UpdateCheckURL"].StringValue = this.UpdateCheckURL;
                initials["UpdateInterval"].IntValue = this.UpdateInterval;
                initials["SDShellPathName"].StringValue = this.SDShellPathName;

                initials.SaveToFile(this.ConfigurationString);
            }
            else
            {
                this.UpdateProcessPath = initials["UpdateProcessPath"].StringValue;
                this.UpdateCheckURL = initials["UpdateCheckURL"].StringValueTrimmed;
                this.UpdateDownloadLocation = initials["UpdateDownloadLocation"].StringValue;
                this.UpdateInterval = initials["UpdateInterval"].IntValue;
                this.SDShellPathName = initials["SDShellPathName"].StringValue;
            }
        }
    }
}
