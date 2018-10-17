using Hydra.Framework.NancyExpand;
using ShanDian.AddIns.Print.Dto.StandardVerSite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.AddIns.Print.Dto.View;

namespace ShanDian.AddIns.Print.Interface.StandardVerSite
{

    [RoutePrefix("StandardVerSite", "Print")]
    public interface IStandardVerSiteService
    {
        /// <summary>
        /// 第三方订制打印
        /// </summary>
        /// <returns></returns>
        [Put("Print/ThirdPart", "第三方订制打印")]
        BaseResult ThirdPartPrint(ThirdPartPrint.Dto thirdPartPrint);

        /// <summary>
        /// 第三方订制打印V2
        /// </summary>
        /// <returns></returns>
        [Put("Print/ThirdPartV2", "第三方订制打印V2")]
        BaseResult ThirdPartPrintV2(ThirdPartPrint.DtoV2 thirdPartPrint);

        /// <summary>
        /// 根据打印方案组获取打印方案(包含打印机信息)列表
        /// </summary>
        /// <param name="printGetInfo"></param>
        /// <returns></returns>
        [Get("Print/ThirdPartV2/PrintConfigList", "根据打印方案组获取打印方案(包含打印机信息)列表")]
        PrintSchemeInfoV2 ThirdPartPrintConfigList(PrintGetInfoDtoV2 printGetInfo);
    }
}
