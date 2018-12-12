using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Data
{
    class ProcessFailedException : Exception
    {
        List<string> progessLog;

        public ProcessFailedException(List<string> progessLog) : this(progessLog, null)
        {
        }

        public ProcessFailedException(List<string> progessLog, Exception ex) : base("Setup process failed.", ex)
        {
            this.progessLog = progessLog;
        }

        public List<string> ProgessLog
        {
            get { return progessLog; }
        }

        public string LastLog
        {
            get { return progessLog.Count > 0 ? progessLog[progessLog.Count - 1] : ""; }
        }
    }
}
