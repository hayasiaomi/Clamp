using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Model
{
    public class SDShellSettings
    {
        public int MainListener { set; get; }
        public string InitializeUrl { set; get; }
        public string AdvicesUrl { set; get; }
        public string CultureName { set; get; }

        public bool ShowDevTools { set; get; }

        public bool PrintProcess { set; get; }

        public bool IgnoreCertificateErrors { set; get; }

        public string ShanDianHost { set; get; }

        public int ShanDianPort { set; get; }
    }
}
