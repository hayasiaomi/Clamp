using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.MqttUtility;
using Hydra.Framework.NancyExpand;
using ShanDian.AddIns.Print.Dto.ScanCode;

namespace ShanDian.AddIns.Print.Interface
{

    [RoutePrefix("", "ScanCode")]
    public interface IScanCodeServices 
    {
        [Get("GetTableInfo", "获取餐桌信息")]
        TableInfoDto GetTableInfo();
    }
}
