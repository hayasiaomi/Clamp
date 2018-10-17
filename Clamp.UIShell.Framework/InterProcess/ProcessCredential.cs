using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.InterProcess
{
    public class ProcessCredential
    {
        public int ProcessId { set; get; }

        public string ProcessName { set; get; }

        public string Parameters { set; get; }

        public string Token { set; get; }
    }
}
