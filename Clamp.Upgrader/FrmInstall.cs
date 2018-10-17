using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.ServiceProcess;
using System.Windows.Forms;
using Clamp.Upgrader.Properties;

namespace Clamp.Upgrader
{
    public partial class FrmInstall : Form
    {
        private BackgroundWorker _backgroundWorker;
        private UpgradeInfo upgradeInfo;

        public FrmInstall(UpgradeInfo upgradeInfo)
        {
            this.upgradeInfo = upgradeInfo;

            InitializeComponent();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (this.upgradeInfo.ExitProcesses != null && this.upgradeInfo.ExitProcesses.Count > 0)
            {
                labelInformation.Text = @"正在关闭相关应用进程...";

                foreach (string processName in this.upgradeInfo.ExitProcesses)
                {
                    try
                    {
                        Process[] exitProcesses = Process.GetProcessesByName(processName);

                        if (exitProcesses != null && exitProcesses.Length > 0)
                        {
                            foreach (Process exitProcess in exitProcesses)
                            {
                                exitProcess.Kill();

                                while (!exitProcess.WaitForExit(3000))
                                {
                                    DialogResult dialogResult = MessageBox.Show(this, $"进程名为{processName}的无法关闭，请手动关闭之后。在重试", "提示", MessageBoxButtons.OKCancel);

                                    if (dialogResult != DialogResult.OK)
                                    {
                                        this.Close();
                                        return;
                                    }
                                }
                            }
                        }

                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                    }

                }
            }

            if (this.upgradeInfo.ExitWindowServices != null && this.upgradeInfo.ExitWindowServices.Count > 0)
            {
                labelInformation.Text = @"正在关闭相关服务进程...";

                foreach (string windowServiceName in this.upgradeInfo.ExitWindowServices)
                {
                    try
                    {
                        while (!this.StopWindowService(windowServiceName))
                        {
                            DialogResult dialogResult = MessageBox.Show(this, $"服务名为{windowServiceName}的无法关闭，请手动关闭之后。在重试", "提示", MessageBoxButtons.OKCancel);

                            if (dialogResult != DialogResult.OK)
                            {
                                this.Close();
                                return;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception.Message);
                    }

                }
            }

            // Extract all the files.
            _backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _backgroundWorker.DoWork += (o, eventArgs) =>
            {
                // Open an existing zip file for reading.
                ZipStorer zip = ZipStorer.Open(this.upgradeInfo.UpgradedFilePath, FileAccess.Read);

                // Read the central directory collection.
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                for (var index = 0; index < dir.Count; index++)
                {
                    if (_backgroundWorker.CancellationPending)
                    {
                        eventArgs.Cancel = true;
                        zip.Close();
                        return;
                    }

                    ZipStorer.ZipFileEntry entry = dir[index];

                    zip.ExtractFile(entry, Path.Combine(this.upgradeInfo.UpgradedTargetPath, entry.FilenameInZip));

                    _backgroundWorker.ReportProgress((index + 1) * 100 / dir.Count, string.Format(Resources.CurrentFileExtracting, entry.FilenameInZip));
                }

                zip.Close();
            };

            _backgroundWorker.ProgressChanged += (o, eventArgs) =>
            {
                progressBar.Value = eventArgs.ProgressPercentage;
                labelInformation.Text = eventArgs.UserState.ToString();
            };

            _backgroundWorker.RunWorkerCompleted += (o, eventArgs) =>
            {
                if (!eventArgs.Cancelled)
                {
                    labelInformation.Text = "更新成功";
                    //try
                    //{
                    //    ProcessStartInfo processStartInfo = new ProcessStartInfo(args[2]);

                    //    if (args.Length > 3)
                    //    {
                    //        processStartInfo.Arguments = args[3];
                    //    }
                    //    Process.Start(processStartInfo);

                    //}
                    //catch (Win32Exception exception)
                    //{
                    //    if (exception.NativeErrorCode != 1223)
                    //        throw;
                    //}

                    Application.Exit();
                }
            };

            _backgroundWorker.RunWorkerAsync();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _backgroundWorker?.CancelAsync();
        }

        private bool ExitProcessByName(Process process)
        {
            process.Kill();

            return process.WaitForExit(3000);
        }
        public static bool IsInstalled(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        public static AssemblyInstaller GetInstaller(string fileName)
        {
            AssemblyInstaller installer = new AssemblyInstaller(fileName, null);

            installer.UseNewContext = true;

            return installer;
        }

        private void UninstallWindowService(string fileName, string serviceName)
        {
            if (!IsInstalled(serviceName))
                return;

            using (AssemblyInstaller installer = GetInstaller(fileName))
            {
                installer.Uninstall(new Hashtable());
            }

        }

        public bool StopWindowService(string serviceName)
        {
            try
            {
                if (!IsInstalled(serviceName))
                    return true;

                using (ServiceController controller = new ServiceController(serviceName))
                {
                    if (controller.Status != ServiceControllerStatus.Stopped)
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return !IsWindowServiceRunning(serviceName);
        }

        public bool IsWindowServiceRunning(string serviceName)
        {
            using (ServiceController controller = new ServiceController(serviceName))
            {
                if (!IsInstalled(serviceName))
                    return false;

                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

    }
}
