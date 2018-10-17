using Clamp.AddIns;
using Clamp.Machine;
using Clamp.Machine.Dto;
using Clamp.Machine.ViewModel;
using Clamp.SDK.Framework;
using Clamp.SDK.Framework.Model;
using Clamp.SDK.Framework.Services;
using Clamp.Webwork;
using Clamp.Webwork.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Machine.Modules
{
    public class MachineController : WebworkController
    {
        private readonly IMachineService machineService;
        public MachineController() : base("Restaurant")
        {
            machineService = AddInManager.GetEntityService<IMachineService>("MachineService");
        }

        [Get("RestInfo")]
        public dynamic GetRestInfo()
        {
            SDResponse<VMStore> response = new SDResponse<VMStore>();

            Store store = machineService.GetStore();

            if (store == null)
            {
                response.Flag = false;
                return null;
            }
            else
            {
                response.Result = new VMStore();

                response.Result.Address = store.Address;
                response.Result.BrandId = store.BrandId;
                response.Result.BrandName = store.BrandName;
                response.Result.MikeRestaurantId = store.MikeRestId;
                response.Result.Name = store.Name;
                response.Result.Phone = store.Phone;
                response.Result.SubName = store.SubName;
            }


            return response.SerializeObject();
        }

        [Patch("RestInfo")]
        public dynamic UpdateRestInfo(VMStore restaurantDto)
        {
            SDResponse<object> response = new SDResponse<object>();

            //if (restaurantDto == null)
            //{
            //    response.Flag = false;
            //    response.Code = MachineCode.Code.ParamsError;

            //    return response;
            //}
            //var restaurantServices = new RestaurantServices();
            //var miRestaurant = GetMiRestaurant(restaurantDto);

            //if (miRestaurant == null)
            //{
            //    response.Flag = false;
            //    response.Code = RestaurantCode.Code.ResultNull;
            //    return response;
            //}

            //var result = restaurantServices.UpdateRestInfo(miRestaurant);

            //response.Flag = result;

            return response.SerializeObject();
        }

        [Put("RestInfo")]
        public dynamic UpdateRestInfo(string name, string value)
        {
            SDResponse<object> response = new SDResponse<object>();

            return response.SerializeObject();
        }

        [Get("Machines/Code")]
        public dynamic GetMachineByCode(string code)
        {
            SDResponse<VMComputer> response = new SDResponse<VMComputer>();

            Computer computer = machineService.GetComputerByCode(code);

            if (computer == null)
            {
                response.Flag = false;
                return null;
            }
            else
            {
                response.Result = new VMComputer();

                response.Result.Code = computer.Code;
                response.Result.Name = computer.Name;
                response.Result.Ip = computer.IpString;
                //response.Result.MainListener = computer.MainListener;
                response.Result.Type = computer.RunMode == "main" ? 10 : 20;
            }

            return response.SerializeObject();

        }

    }
}
