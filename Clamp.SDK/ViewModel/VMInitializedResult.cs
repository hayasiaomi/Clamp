using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.ViewModel
{
    public class VMInitializedResult
    {
        public bool IsInitialized { set; get; }

        public string PCID { set; get; }
        public string AppId { set; get; }

        public string SecureKey { set; get; }

        public string MikeRestId { set; get; }

        public string RunMode { set; get; }

        public string Server { set; get; }

        public string ErrorMessage { set; get; }
    }
}
