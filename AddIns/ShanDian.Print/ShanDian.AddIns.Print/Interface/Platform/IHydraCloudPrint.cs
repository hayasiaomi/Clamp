using Hydra.Framework.MqttUtility;
using Hydra.Framework.NancyExpand;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.Platform;
using ShanDian.AddIns.Print.Dto.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.Services.HttpUtility;

namespace ShanDian.AddIns.Print.Interface.Platform
{

    [RoutePrefix("", "api")]
    public interface IHydraCloudPrint : IHyperServer
    {
        //[Get("PrintConfigPage", "打印配置分页")]
        //CloudOutPage<PrintConfigPageVDto> GetPrintConfigPage(int pageIndex, int pageSize);

        //[Get("PrintConfig", "获取打印配置信息")]
        //PrintConfigDto GetPrintConfig(string pcid, string printName);

        [Delete("PrintConfig", "删除打印配置", async: true)]
        void DeletePrintConfig(string pcid, string printName);

        [Put("PrintConfig", "更新打印配置", async: true)]
        void UpdatePrintConfig(PrintConfigDto printConfigDto);

        [Post("PrintConfig", "添加打印配置", async: true)]
        void AddPrintConfig(PrintConfigDto printConfigDto);

        //[Get("PrintPage", "打印机分页")]
        //CloudOutPage<PrintInfoVDto> GetPrintPage(string pcid, int pageIndex, int pageSize);

        //[Post("PrintInfo", "添加打印机")]
        //void AddPrintInfo(string pcid, string printName, int state);

        //[Put("PrintInfo", "更新打印机")]
        //void UpdatePrintInfo(string pcid, string printName, int state);

    }
}
