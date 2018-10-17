using Newtonsoft.Json;
using ShanDian.UIShell.Framework.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using static System.Management.ManagementObjectCollection;

namespace ShanDian.UIShell.Framework.InterProcess
{
    public class ProcessWraper
    {
        private Process process;

        public string ProcessFilename { private set; get; }

        public bool Locked { set; get; }

        public event EventHandler ProcessExited;

        public ProcessWraper(string processFilename) : this(processFilename, false)
        {

        }

        public ProcessWraper(string processFilename, bool locked)
        {
            this.ProcessFilename = processFilename;
            this.Locked = locked;
        }


        public void Open(string data = "")
        {
            if (this.process == null)
            {
                string processName = Path.GetFileNameWithoutExtension(this.ProcessFilename);

                Process[] runProcesses = Process.GetProcessesByName(processName);

                if (runProcesses != null && runProcesses.Length > 0)
                {
                    foreach (Process runProcess in runProcesses)
                    {
                        string filename = this.GetProcessExecutablePath(runProcess);

                        if (string.Equals(filename, this.ProcessFilename, StringComparison.CurrentCultureIgnoreCase))
                        {
                            runProcess.Kill();

                            runProcess.WaitForExit(3000);
                        }
                    }
                }

                Process currentProcess = Process.GetCurrentProcess();

                ProcessCredential processCredential = new ProcessCredential();

                processCredential.ProcessName = currentProcess.ProcessName;
                processCredential.ProcessId = currentProcess.Id;
                processCredential.Parameters = data;
                processCredential.Token = "ShanDian";

                this.process = new Process();

                ProcessStartInfo psi = new ProcessStartInfo(this.ProcessFilename);

                string dataString = JsonConvert.SerializeObject(processCredential);

                string cmdLine = Convert.ToBase64String(Encoding.UTF8.GetBytes(dataString));

                NLogService.Info(cmdLine);

                psi.UseShellExecute = false;
                psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                psi.Arguments = cmdLine;

                this.process.StartInfo = psi;
                this.process.EnableRaisingEvents = true;
                if (this.Locked)
                    this.process.Exited += this.Process_Exited;

                this.process.Start();
                this.process.WaitForInputIdle(3000);
            }

        }

        public void Exit()
        {
            if (this.process != null)
                this.process.Exited -= this.Process_Exited;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (this.ProcessExited != null)
            {
                this.ProcessExited(sender, e);
            }
        }

        private string GetProcessExecutablePath(Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                string query = $"SELECT ExecutablePath, ProcessID FROM Win32_Process WHERE ProcessID={ process.Id.ToString()}";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                ManagementObjectCollection objectCollection = searcher.Get();

                if (objectCollection != null && objectCollection.Count > 0)
                {
                    foreach (ManagementObject item in objectCollection)
                    {
                        object id = item["ProcessID"];
                        object path = item["ExecutablePath"];

                        if (path != null && id.ToString() == process.Id.ToString())
                        {
                            return path.ToString();
                        }
                    }
                }
            }

            return "";
        }
    }
}
