using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Upgrader
{
    public class UpgradeInfo
    {
        public string Name { set; get; }

        public List<string> ExitWindowServices { set; get; }

        public List<string> ExitProcesses { set; get; }

        public string VersionCode { set; get; }

        public string UpgradedFilePath { set; get; }

        public string UpgradedTargetPath { set; get; }
    }
}
