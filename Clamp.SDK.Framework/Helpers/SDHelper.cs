using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clamp.SDK.Framework.Helpers
{
    public class SDHelper
    {
        private static SDDemand demandCache;


        /// <summary>
        /// 装载激活的凭证信息,存在文件
        /// </summary>
        /// <returns></returns>
        public static void UnInstallDemandFile()
        {
            if (demandCache != null)
            {
                demandCache = null;

            }

            string demandFile = Path.Combine(GetSDRootPath(), SDDemand.DemandFileName);

            int deleteTimer = 0;

            while (File.Exists(demandFile) && deleteTimer < 3)
            {
                File.Delete(demandFile);
                Thread.Sleep(50);
                deleteTimer++;
            }
        }

        /// <summary>
        /// 装载激活的凭证信息,存在文件
        /// </summary>
        /// <returns></returns>
        public static void SetupDemandFile(SDDemand demand)
        {
            if (demand != null)
            {
                string demandFile = Path.Combine(GetSDRootPath(), SDDemand.DemandFileName);
                string demandJson = JsonConvert.SerializeObject(demand);
                string demandDir = Path.GetDirectoryName(demandFile);

                if (!Directory.Exists(demandDir))
                    Directory.CreateDirectory(demandDir);

                File.WriteAllText(demandFile, demandJson, Encoding.UTF8);
            }
        }

        /// <summary>
        /// 装载激活的凭证信息，放到内存
        /// </summary>
        /// <returns></returns>
        public static void SetupDemandMemory(SDDemand demand)
        {
            if (demand != null)
            {
                demandCache = demand;
            }
        }

        /// <summary>
        /// 获得激活的凭证信息
        /// </summary>
        /// <returns></returns>
        public static bool IsActivited()
        {
            SDDemand demand = GetDemand();

            if (demand != null)
            {
                if (string.Equals("main", demand.RunMode, StringComparison.CurrentCultureIgnoreCase))
                    return !string.IsNullOrWhiteSpace(demand.AppId) && !string.IsNullOrWhiteSpace(demand.SecureKey);
                else if (string.Equals("sub", demand.RunMode, StringComparison.CurrentCultureIgnoreCase))
                    return !string.IsNullOrWhiteSpace(demand.Server);
            }

            return false;
        }

        /// <summary>
        /// 获得激活的凭证信息
        /// </summary>
        /// <returns></returns>
        public static bool IsMain()
        {
            SDDemand demand = GetDemand();

            if (demand != null)
            {
                return !string.IsNullOrWhiteSpace(demand.AppId) && !string.IsNullOrWhiteSpace(demand.SecureKey) && string.Equals(demand.RunMode, "main", StringComparison.CurrentCultureIgnoreCase);
            }

            return false;
        }

        /// <summary>
        /// 获得激活的凭证信息
        /// </summary>
        /// <returns></returns>
        public static SDDemand GetDemand()
        {
            if (demandCache == null)
            {
                string demandFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SDDemand.DemandFileName);

                if (File.Exists(demandFile))
                {
                    string demandJson = File.ReadAllText(demandFile, Encoding.UTF8);

                    demandCache = JsonConvert.DeserializeObject<SDDemand>(demandJson);
                }
            }

            return demandCache;
        }


        /// <summary>
        /// 获得应用的根目录
        /// </summary>
        /// <returns></returns>
        public static string GetSDRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
