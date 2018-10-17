using ShanDian.Common.Model;
using ShanDian.SDK.Model;
using ShanDian.SDK.Framework.SDAPI;
using ShanDian.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.SDK.Framework.Model;

namespace ShanDian.SDK.Services
{
    public interface IInstallService : IService
    {
        /// <summary>
        /// 获得注册表的GUID。
        /// </summary>
        /// <returns></returns>
        string GetRegistryPCID();

        /// <summary>
        /// 保存PICD到注册表
        /// </summary>
        /// <param name="pcid"></param>
        void SaveRegistryPCID(string pcid);

        /// <summary>
        /// 保存门店编号
        /// </summary>
        /// <param name="pcid"></param>
        void SaveMerchantNo(string merchantNo);

        /// <summary>
        /// 激活门店
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        SDApiResponse<SDApiDemand> ActivitedStore(string storeId);

        /// <summary>
        /// 用于UI的安装分机
        /// </summary>
        /// <param name="mainIp"></param>
        /// <returns></returns>
        SetupSubResult SetupSub(string mainIp);

        /// <summary>
        /// 只有主机才有的安装分机
        /// </summary>
        /// <param name="mainIp"></param>
        /// <returns></returns>
        BegInstallSubResult BegInstallSub(string host, string pcid, string subIp, int subListener);

        /// <summary>
        /// 只有主机才有的安装分机
        /// </summary>
        /// <param name="mainIp"></param>
        /// <returns></returns>
        InstallSubResult InstallSub(string pcid, string subIp, int subListener);
    }
}
