using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.LSRestaurant.Interface.Dto.Com;
using ShanDian.LSRestaurant.Interface.Dto.Dishes;
using ShanDian.SDK.Framework.Model;
using ShanDian.SDK.Framework.Services;

namespace ShanDian.LSRestaurant.Interface.Interface
{

    public interface IDishesServices : IService
    {
        #region 点菜

        //[Get("menu/dishes/page", "获取点菜的菜品分页列表")]
        SDApiResponse<PageListDto<DishMenuPageDto>> GetMenuDishesPage(DishQueryDto queryDto);

        //[Get("menu/dish/id", "获取点菜的菜品详情")]
        //SDApiResponse<DishMenuDetailDto> GetMenuDishById(string id);

        //[Get("menu/childDish/practices/id", "获取菜品做法")]
        //SDApiResponse<List<PracticeDetailDto>> GetMenuChildDishPracticesById(string id);

        //[Get("menu/childDish/chargingFoods/id", "获取菜品的加料辅菜")]
        //List<MenuDishGroupDto> GetMenuChildDishChargingFoodsById(string id);

        //[Get("menu/estimate/dishesPage", "获取估清菜品分页列表")]
        //PageListDto<EstimateDishDto> GetMenuEstimateDishesPage(QueryDto queryDto);

        //[Get("menu/estimate/dish/ids", "获取估清菜品全部id")]
        //List<string> GetMenuEstimateDishIds();

        //[Get("menu/Estimate/dishes", "获取估清菜品")]
        //List<EstimateDishDto> GetMenuEstimateDishes();

        //[Post("menu/estimate/dish", "新加估清菜品")]
        //void AddEstimateDish(EstimateDishInDto estimateDishInDto);

        //[Put("menu/estimate/dish", "修改估清菜品")]
        //void AlterEstimateDish(EstimateDishInDto estimateDishInDto);

        ////[Put("menu/estimate/dishes", "估清使用的菜品")]
        ////void EstimateDishes(EstimateDishSumInDto estimateDishSumInDto);

        //[Put("menu/estimate/dishes", "估清使用的菜品")]
        //void EstimateDishes(List<EstimateDishDto> estimateDishDtoList);

        //[Delete("menu/estimate/dish", "删除估清菜品")]
        //void DeleteEstimateDishesByIds(DeleteInDto deleteInDto);

        //[Delete("menu/estimate/allDish", "删除全部估清菜品")]
        //void DeleteAllEstimateDishes(InputDto inputDto);



        #endregion
    }
}
