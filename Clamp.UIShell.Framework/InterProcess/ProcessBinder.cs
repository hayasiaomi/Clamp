using Clamp.UIShell.Framework.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clamp.UIShell.Framework.InterProcess
{
    public class ProcessBinder
    {
        private BackgroundWorker bindProcessRunningWorker;
        private Process bindProcess;

        public int ProcessId { private set; get; }

        public event EventHandler ProcessExited;

        public ProcessBinder(int processId)
        {
            this.ProcessId = processId;

            NLogService.Info("BIND进程ID：" + this.ProcessId);
        }

        public void Bind()
        {
            if (!this.BindMainProcess())
            {
                NLogService.Info("没有成功BIND 用后台线程");

                if (this.bindProcessRunningWorker == null)
                {
                    this.bindProcessRunningWorker = new BackgroundWorker();
                    this.bindProcessRunningWorker.DoWork += BindProcessRunningWorker_DoWork;
                    this.bindProcessRunningWorker.WorkerSupportsCancellation = true;
                    this.bindProcessRunningWorker.RunWorkerAsync();
                }
            }

            NLogService.Info("成功BIND 用退出事件");

        }

        private bool BindMainProcess()
        {
            try
            {
                this.bindProcess = Process.GetProcessById(this.ProcessId);

                if (this.bindProcess != null)
                {
                    this.bindProcess.WaitForInputIdle(1000);

                    this.bindProcess.EnableRaisingEvents = true;
                    this.bindProcess.Exited += BindProcess_Exited;

                    return true;
                }
            }
            catch (Exception ex)
            {
                NLogService.Error("绑定主机进程失败", ex);
            }

            return false;
        }

        private void BindProcessRunningWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                if (!this.BindMainProcess())
                {
                    Process SDShellProcess = Process.GetProcessById(this.ProcessId);

                    if (SDShellProcess != null)
                    {
                        Thread.Sleep(500);

                        continue;
                    }
                    else
                    {
                        if (this.ProcessExited != null)
                            this.ProcessExited(this, new EventArgs());
                    }
                }

                break;
            }
        }

        private void BindProcess_Exited(object sender, EventArgs e)
        {
            if (this.ProcessExited != null)
                this.ProcessExited(sender, e);
        }
    }
}
