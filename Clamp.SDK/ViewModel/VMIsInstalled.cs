using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.ViewModel
{
    public class VMIsInstalled
    {
        public string Version { set; get; }

        public bool IsInstalled { set; get; }

        public string RunMode { set; get; }
    }
}
