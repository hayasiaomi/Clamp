using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Model
{
    public class InitializedInfo
    {
        public bool IsInitialized { set; get; }

        public string PCID { set; get; }
        public string AppId { set; get; }

        public string SecureKey { set; get; }

        public string Name { set; get; }

        public string MikeRestId { set; get; }

        public string SubName { set; get; }

        public string BrandId { set; get; }

        public string BrandName { set; get; }

        public string Address { set; get; }

        public string Phone { set; get; }

        public string Logo { set; get; }

        public string RunMode { set; get; }

        public string Server { set; get; }

        public string ErrorMessage { set; get; }
    }
}
