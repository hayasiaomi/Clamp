using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.MUI
{
    public class ChildProcess
    {
        private NamedPipeClientStream namedPipeClientStream;
        private BackgroundWorker ReadBackgroundWorker;
        private Process process;
        private bool loopReceived = true;
        private bool IsExited = false;

        public event Action<string> OnReceived;

        public event Action<Process> OnFloatExisted;

        public string ProcessName { private set; get; }

        public ChildProcess(string processName)
        {
            this.ProcessName = processName;

        }
        public void Open()
        {
            this.IsExited = false;

            Task.Factory.StartNew(this.OpenProcess);
        }

        public void Exit()
        {
            this.IsExited = true;
            this.loopReceived = false;

            if (this.process != null)
            {
                if (!process.HasExited)
                {
                    if (this.namedPipeClientStream != null)
                    {
                        this.WriteLineAsync("ExitProcess");
                    }
                }

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

            if (this.namedPipeClientStream != null)
            {
                this.namedPipeClientStream.Close();
                this.namedPipeClientStream.Dispose();
                this.namedPipeClientStream = null;
            }

            if (this.ReadBackgroundWorker != null)
            {
                if (this.ReadBackgroundWorker.IsBusy)
                    this.ReadBackgroundWorker.CancelAsync();

                this.ReadBackgroundWorker.Dispose();
                this.ReadBackgroundWorker = null;
            }
        }

        private void OpenProcess()
        {
            this.loopReceived = true;

            string mark = "Floating" + new Random().Next(1, 10);

            if (this.process == null)
            {
                process = new Process();

                ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.ProcessName));

                psi.UseShellExecute = false;
                psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                psi.Arguments = "Clamp " + mark;

                process.StartInfo = psi;
                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;
                process.Start();

                process.WaitForInputIdle(3000);
            }

            if (this.namedPipeClientStream == null)
                this.namedPipeClientStream = new NamedPipeClientStream(".", mark, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

            if (!this.namedPipeClientStream.IsConnected)
                this.namedPipeClientStream.Connect();

            if (this.ReadBackgroundWorker == null)
            {
                this.ReadBackgroundWorker = new BackgroundWorker();
                this.ReadBackgroundWorker.DoWork += ReadBackgroundWorker_DoWork;
                this.ReadBackgroundWorker.WorkerSupportsCancellation = true;
                this.ReadBackgroundWorker.RunWorkerAsync();
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (this.OnFloatExisted != null)
                this.OnFloatExisted(this.process);
        }

        private void ReadBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (this.loopReceived)
            {
                try
                {
                    string message = this.ReadLine();

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        if (message == "ExitProcess")
                        {
                            this.loopReceived = false;
                            break;
                        }
                        else
                        {
                            if (this.OnReceived != null)
                                this.OnReceived(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.namedPipeClientStream != null && !this.namedPipeClientStream.IsConnected)
                    {
                        if (!this.IsExited)
                        {
                            this.loopReceived = false;
                            break;
                        }
                        else
                        {
                            this.namedPipeClientStream.Connect();
                        }
                    }
                }
            }
        }

        public void WriteLine(string message)
        {
            try
            {
                if (this.namedPipeClientStream != null && this.namedPipeClientStream.IsConnected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(message);

                    var lenbuf = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(buffer.Length));

                    this.namedPipeClientStream.Write(lenbuf, 0, lenbuf.Length);
                    this.namedPipeClientStream.Flush();
                    this.namedPipeClientStream.WaitForPipeDrain();
                    this.namedPipeClientStream.Write(buffer, 0, buffer.Length);
                    this.namedPipeClientStream.Flush();
                    this.namedPipeClientStream.WaitForPipeDrain();
                }
            }
            catch (Exception)
            {

            }
        }

        public void WriteLineAsync(string message)
        {
            Task.Factory.StartNew(this.WriteMessageTask, message);
        }

        private void WriteMessageTask(object state)
        {
            this.WriteLine(Convert.ToString(state));
        }

        private string ReadLine()
        {
            int lensize = sizeof(int);
            byte[] lenbuf = new byte[lensize];
            this.namedPipeClientStream.Read(lenbuf, 0, lensize);

            int len = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lenbuf, 0));

            byte[] buffer = new byte[len];

            this.namedPipeClientStream.Read(buffer, 0, len);

            return Encoding.UTF8.GetString(buffer);
        }
    }
}
