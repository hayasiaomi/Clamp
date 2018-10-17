using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShanDian.UIShell.Framework.InterProcess
{
    public class ProcessMonitor
    {
        private NamedPipeClientStream mPipeStream;
        private BackgroundWorker ReadBackgroundWorker;
        private Process process;

        private bool loopReceived = true;


        public string ProcessName { private set; get; }

        public event ProcessMonitorExist OnProcessMonitorExist;
        public event ProcessMonitorStart OnProcessMonitorStart;
        public event ProcessMonitorReceived OnProcessMonitorReceived;

        public bool IsExited { private set; get; }

        public ProcessMonitor(string processName)
        {
            this.ProcessName = processName;
        }

        public void Start(string message = "")
        {
            this.loopReceived = true;

            string pipeMark = string.Format("M{0}{1}", Path.GetFileNameWithoutExtension(this.ProcessName), new Random().Next(1, 10));

            if (this.process == null)
            {
                this.process = new Process();

                ProcessStartInfo psi = new ProcessStartInfo(this.ProcessName);

                string cmdLine = Convert.ToBase64String(Encoding.UTF8.GetBytes(message));

                psi.UseShellExecute = false;
                psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                psi.Arguments = string.Format("Hydra {0} {1}", pipeMark, cmdLine);

                this.process.StartInfo = psi;
                this.process.EnableRaisingEvents = true;
                this.process.Exited += Process_Exited;

                this.process.Start();
                this.process.WaitForInputIdle(3000);


                if (this.OnProcessMonitorStart != null)
                    this.OnProcessMonitorStart(this, new EventArgs());

            }

            if (this.mPipeStream == null)
                this.mPipeStream = new NamedPipeClientStream(".", pipeMark, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            if (!this.mPipeStream.IsConnected)
                this.mPipeStream.Connect();

            if (this.ReadBackgroundWorker == null)
            {
                this.ReadBackgroundWorker = new BackgroundWorker();
                this.ReadBackgroundWorker.DoWork += ReadBackgroundWorker_DoWork;
                this.ReadBackgroundWorker.WorkerSupportsCancellation = true;
                this.ReadBackgroundWorker.RunWorkerAsync();
            }
        }

        public void StartAsync(string message = "")
        {
            Task.Factory.StartNew(() =>
            {
                this.Start(message);
            });
        }

        public void Exit()
        {
            this.IsExited = true;

            if (this.process != null)
            {
                if (!process.WaitForExit(2000))
                {
                    process.Kill();
                }

                if (this.process != null)
                {
                    this.process.Dispose();
                    this.process = null;
                }
            }

            this.ShutdownNamedPipe();
        }

        private void ReadBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (this.loopReceived)
            {
                try
                {
                    string message = this.ReadLine();

                    if (this.OnProcessMonitorReceived != null)
                        this.OnProcessMonitorReceived(this, new ProcessMonitorReceivedEventArgs(message));
                }
                catch (Exception ex)
                {
                    if (!this.IsExited)
                    {
                        this.loopReceived = false;
                        break;
                    }
                    else
                    {
                        if (this.mPipeStream != null && !this.mPipeStream.IsConnected)
                        {
                            this.mPipeStream.Connect();
                        }
                    }
                }
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            this.process.WaitForExit();

            if (this.process != null)
            {
                this.process.Dispose();
                this.process = null;
            }

            this.ShutdownNamedPipe();

            if (this.OnProcessMonitorExist != null)
                this.OnProcessMonitorExist(sender, e);
        }

        private void ShutdownNamedPipe()
        {
            this.loopReceived = false;

            if (this.mPipeStream != null)
            {
                this.mPipeStream.Close();
                this.mPipeStream.Dispose();
                this.mPipeStream = null;
            }

            if (this.ReadBackgroundWorker != null)
            {
                if (this.ReadBackgroundWorker.IsBusy)
                    this.ReadBackgroundWorker.CancelAsync();

                this.ReadBackgroundWorker.Dispose();
                this.ReadBackgroundWorker = null;
            }
        }


        /// <summary>
        /// 读取子进程发来的信息
        /// </summary>
        /// <returns></returns>
        private string ReadLine()
        {
            int lensize = sizeof(int);
            byte[] lenbuf = new byte[lensize];
            this.mPipeStream.Read(lenbuf, 0, lensize);

            int len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenbuf, 0));

            byte[] buffer = new byte[len];

            this.mPipeStream.Read(buffer, 0, len);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
