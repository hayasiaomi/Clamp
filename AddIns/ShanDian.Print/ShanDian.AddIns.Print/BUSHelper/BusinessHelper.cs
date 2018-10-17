using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.AddIns.Print.Dto.Restaurant;
using ShanDian.AddIns.Print.Interface.Platform;
using ShanDian.AddIns.Print.Model;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.AddIns.Print.Services.BUSHelper
{
    public class BusinessHelper
    {
        #region init
        private BusinessHelper() { }

        private static BusinessHelper _instance;
        private bool _systemModeChange;
        private IRepositoryContext _repositoryContext;
        public RestaurantsInfo _RestaurantsInfo;

        public static BusinessHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BusinessHelper();
                    _instance._repositoryContext = Global.RepositoryContext();
                    _instance._RestaurantsInfo = new RestaurantsInfo();
                    _instance._RestaurantsInfo = _instance.GetRestaurantsInfo();
                }
                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// 获取餐厅信息
        /// </summary>
        /// <param name="source">内存中使用</param>
        /// <returns></returns>
        public RestaurantsInfo GetRestaurantsInfo(int source = 1)
        {
            RestaurantsInfo RestInfoCfg = null;
            if (source == 1)
            {
                var sdApiServices = CloudLoader.Load<ISDApiServices>();
                var restInfo = sdApiServices.GetRestInfo();
                DataBaseLog.Writer.SendInfo($"[餐厅配置信息]{restInfo.ToJson()}");
                if (sdApiServices.Flag && restInfo != null)
                {
                    RestInfoCfg = restInfo;
                }
            }
            else
            {
                RestaurantsInfo restInfo = new CloudLoader().CallHttp<RestaurantsInfo>($"{HydraSystemConfig.SdApiHost}/v1/rests/scancode", HttpMethod.Get, "");
                if (restInfo != null)
                {
                    RestInfoCfg = restInfo;
                }
            }
            return RestInfoCfg;
        }

        public bool RestSystemMode()
        {
            bool restFlag = true;
            restFlag = HydraSystemConfig.IsSystemModeChanged;
            if (restFlag)
            {
                try
                {
                    var restTypeInfo = HydraSystemConfig.CombineRestType;

                    switch (restTypeInfo)
                    {
                        case 20:
                            DataBaseLog.Writer.SendInfo("餐厅的模式切换成功,当前系统对接信息为 -> 对接第三方餐饮系统");
                            break;
                        case 30:
                            DataBaseLog.Writer.SendInfo("餐厅的模式切换成功,当前系统对接信息为 -> 对接善点轻餐系统");
                            break;
                        default:
                            DataBaseLog.Writer.SendInfo("餐厅的模式切换成功,当前系统对接信息为 -> 默认非对接系统");
                            break;
                    }

                    //删除所有的餐桌数据信息
                    //_repositoryContext.Execute("delete from tb_printSchemeLabel Where Id != @Id", new { Id = 0 });
                    //_repositoryContext.Execute("delete from tb_schemeDishType Where Id != @Id", new { Id = 0 });

                    DataBaseLog.Writer.SendInfo("餐厅的模式切换成功,已删除所有的参数的数据信息");
                }
                catch (Exception e)
                {
                    restFlag = false;
                    DataBaseLog.Writer.SendInfo("餐厅的模式切换失败,失败信息：" + e.ToString());
                }
            }

            return restFlag;
        }
    }
}
