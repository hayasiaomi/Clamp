using ShanDian.AddIns;
using ShanDian.LSRestaurant.ErrorCode;
using ShanDian.LSRestaurant.Interface.Dto.Com;
using ShanDian.LSRestaurant.Interface.Dto.Dishes;
using ShanDian.LSRestaurant.Interface.Interface;
using ShanDian.SDK.Framework;
using ShanDian.SDK.Framework.Model;
using ShanDian.Webwork;
using ShanDian.Webwork.Annotation;

namespace ShanDian.LSRestaurant.Controllers
{
    class DishesController : WebworkController
    {
        private readonly IDishesServices _dishesServices;

        public DishesController() : base("Dishes")
        {
            _dishesServices = AddInManager.GetEntityService<IDishesServices>("DishesServices");
        }

        [Get("menu/dishes/page")] //, "获取点菜的菜品分页列表"
        public dynamic GetMenuDishesPage(DishQueryDto queryDto)
        {
            SDResponse<PageListDto<DishMenuPageDto>> response = new SDResponse<PageListDto<DishMenuPageDto>>();
            if (queryDto == null)
            {
                response.Flag = false;
                response.Code = DishesErrorCode.Code.ParamsError;
                response.Message = "参数为空";
                return null;
            }

            SDApiResponse<PageListDto<DishMenuPageDto>> apiResponse = _dishesServices.GetMenuDishesPage(queryDto);
            response.Result = apiResponse.Data;
            response.Flag = apiResponse.IsSuccess();
            response.Code = apiResponse.Code;
            response.Message = apiResponse.Msg;

            return response.SerializeObject();
        }
    }
}
