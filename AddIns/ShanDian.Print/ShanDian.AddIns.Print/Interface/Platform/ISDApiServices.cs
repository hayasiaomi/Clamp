using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.NancyExpand;
using Hydra.Framework.Services.HttpUtility;
using ShanDian.AddIns.Print.Dto.Restaurant;

namespace ShanDian.AddIns.Print.Interface.Platform
{
    [RoutePrefix("", "", "")]
    public interface ISDApiServices : IHyperServer
    {
        /// <summary>
        /// 获取餐厅信息
        /// </summary>
        [Get("/v1/rests/scancode")]
        RestaurantsInfo GetRestInfo();
    }
}
