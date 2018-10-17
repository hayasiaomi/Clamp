using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Network.Service
{
    public class InitConfigInfo
    {
        public bool Complete { set; get; }
        public string AppId { set; get; }
        public string OrgExtCode { set; get; }
        public string OrgName { set; get; }
        public string OrgSubName { set; get; }

        public string SecureKey { set; get; }

        public bool IsNewRestId { set; get; }
    }
}
