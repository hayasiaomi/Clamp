using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.MqttUtility;
using Hydra.Framework.NancyExpand;
using ShanDian.AddIns.Print.Dto.Restaurant;

namespace ShanDian.AddIns.Print.Interface
{
    [RoutePrefix("", "Restaurant")]
    public interface IRestServices 
    {
        [Get("RestInfo", "获取餐厅信息")]
        RestaurantDto GetRestInfo();

        [Get("Machines", "获取机器")]
        List<MachineDto> GetAllMachines();

        [Post("DeleteTableByScanCode", "删除餐桌数据")]
        void DeleteTableByScanCode();
    }
}
