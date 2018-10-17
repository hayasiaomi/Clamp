using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.UIShell.Framework.Services
{
    public sealed class MachineService
    {


        public static bool UpLoadMachine(string code, string localIp, int typeValue)
        {
            //TODO 用于上传机子本身的信息一般用于分机
            return false;
        }

        public static void UploadSoftware()
        {
            //try
            //{
            //    SoftwareInfo softwareInfo = new SoftwareInfo();

            //    softwareInfo.CurrentVersion = RevisionClass.FullVersion;
            //    softwareInfo.PCID = UIShellSettings.Demand.PCID;
            //    softwareInfo.SystemBit = Environment.Is64BitOperatingSystem ? "64G" : "32G";
            //    softwareInfo.RestSystem = ServerHelper.GetRestSystem();
            //    softwareInfo.IsAutoCheckout = ServerHelper.GetAutoCheckout();
            //    softwareInfo.OperationSystem = OSHelper.Name + " " + OSHelper.Edition;
            //    softwareInfo.MainIp = GetLocalIPAddress();
            //    softwareInfo.CPU = HardwareHelper.GetCPUName();
            //    softwareInfo.Memory = HardwareHelper.GetPhysicalMemory();

            //    ServiceResult<List<MachineInfo>> srMachines = ServiceAccessor.GetAllMachines();

            //    if (srMachines == null || !srMachines.Flag)
            //        softwareInfo.ExtensionCount = -1;
            //    else
            //        softwareInfo.ExtensionCount = srMachines.Result == null ? 0 : srMachines.Result.Count(t => t.Type == 20);

            //    softwareInfo.InstallFolder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
            //    softwareInfo.DiskSpace = new DriveInfo(Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory)).AvailableFreeSpace / 1024 + "MB";

            //    int timer = 0;

            //    while (timer++ < 3 && ApiAccessor.UploadSoftware(softwareInfo).Code != 0) ;
            //}
            //catch (Exception ex)
            //{
            //    DebugHelper.WriteException(ex);
            //}
        }
    }
}
