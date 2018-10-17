using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Clamp.UIShell
{
    public class WinFormClientService
    {
        public const string FileName = "ShanDianWinForm.exe";

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
            if (SDShell.ChromiumWindow != null && SDShell.SDShellSettings.PrintProcess)
            {
                this.OpenWinformClientService();
            }
        }

        private void OpenWinformClientService()
        {
            Process[] processes = Process.GetProcessesByName("ShanDianWinForm");

            if (processes == null || processes.Length == 0)
            {
                this.winFormProcess = new Process();

                ProcessStartInfo psi = new ProcessStartInfo(this.ProcessName);

                psi.UseShellExecute = false;
                psi.WorkingDirectory = SDShell.SDRootPath;
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

        public void DeleteAutoStart()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {

                    if (shanDianRegistry.GetValue("ShanDianClientService") != null)
                    {

                        shanDianRegistry.DeleteValue("ShanDianClientService");
                    }
                }
            }
            else
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (shanDianRegistry.GetValue("ShanDianClientService") != null)
                    {

                        shanDianRegistry.DeleteValue("ShanDianClientService");
                    }
                }
            }
        }

        public void AutoStart(string launch)
        {
            if (Environment.Is64BitOperatingSystem)
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (shanDianRegistry.GetValue("ShanDianClientService") == null)
                    {
                        shanDianRegistry.SetValue("ShanDianClientService", @"""" + launch + @"""");
                    }
                }
            }
            else
            {
                using (var localKey32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                using (var shanDianRegistry = localKey32.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (shanDianRegistry.GetValue("ShanDianClientService") == null)
                    {

                        shanDianRegistry.SetValue("ShanDianClientService", @"""" + launch + @"""");
                    }
                }
            }

        }
    }
}
