using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clamp.MUI
{
    public class WinFormClientService
    {
        private Process winFormProcess;

        public string ProcessName { private set; get; }

        public WinFormClientService(string processName)
        {

            this.ProcessName = processName;
        }

        public void Start()
        {
            this.OpenWinformClientService();
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Thread checkExitThread = new Thread(new ThreadStart(this.CheckMainProcess));

            checkExitThread.Start();
        }

        private void CheckMainProcess()
        {
            //if (ChromiumSettings.ChromiumWindow != null && ChromiumSettings.IsWinformService)
            //{
            //    this.OpenWinformClientService();
            //}
        }

        private void OpenWinformClientService()
        {
            Process[] processes = Process.GetProcessesByName("ShanDianClientService");

            if (processes == null || processes.Length == 0)
            {
                this.winFormProcess = new Process();

                ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, this.ProcessName));

                psi.UseShellExecute = false;
                psi.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                psi.Arguments = "Hydra";


                this.winFormProcess.StartInfo = psi;
                this.winFormProcess.EnableRaisingEvents = true;
                this.winFormProcess.Exited += Process_Exited;
                this.winFormProcess.Start();

                this.winFormProcess.WaitForInputIdle(3000);
            }
            else
            {
                this.winFormProcess = processes[0];

                this.winFormProcess.EnableRaisingEvents = true;
                this.winFormProcess.Exited += Process_Exited;
            }
        }
    }
}
