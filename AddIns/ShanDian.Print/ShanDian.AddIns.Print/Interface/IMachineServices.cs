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
    public interface IMachineServices 
    {
        [Get("Machines/Code", "根据机器码获取机器")]
        MachineDto GetMachineByCode(string code);

        [Get("Machines", "获取机器")]
        List<MachineDto> GetAllMachines();
    }
}
