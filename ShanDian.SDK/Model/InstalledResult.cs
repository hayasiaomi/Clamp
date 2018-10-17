using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Model
{
    public class InstalledResult
    {
        public string Version { set; get; }

        public bool IsInstalled { set; get; }

        public string RunMode { set; get; }

        public string Mistake { set; get; }
    }
}
