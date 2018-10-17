using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.InterProcess
{
    public delegate void ProcessMonitorExist(object sender, EventArgs e);
    public delegate void ProcessMonitorStart(object sender, EventArgs e);
    public delegate void ProcessMonitorReceived(object sender, ProcessMonitorReceivedEventArgs e);

    public class ProcessMonitorReceivedEventArgs : EventArgs
    {
        public string Message { private set; get; }

        public ProcessMonitorReceivedEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
