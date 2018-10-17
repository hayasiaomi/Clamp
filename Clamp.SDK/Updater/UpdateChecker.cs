using Clamp.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Clamp.SDK.Updater
{
    internal abstract class UpdateChecker
    {
 
        public Version CurrentVersion { get; set; }
        public Version LatestVersion { get; set; }
        public bool IsBeta { get; set; }
        public bool IsPortable { get; set; }
        public IWebProxy Proxy { get; set; }

        public UpdateInfo UpdateInfo { set; get; }

        public string ProcessPath { set; get; }
        public string CheckURL { set; get; }
        public string UpdateDownloadPath { set; get; }

        private const bool forceUpdate = false; // For testing purposes

        public abstract UpdateStatus CheckUpdate();

        public virtual void DownloadUpdate()
        {

        }
    }
}
