using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.ErrorCode;
using ShanDian.LSRestaurant.Factories;
using ShanDian.LSRestaurant.Interface.Dto.Com;
using ShanDian.LSRestaurant.Interface.Dto.Dishes;
using ShanDian.LSRestaurant.Interface.Interface;
using ShanDian.LSRestaurant.Model.Dishes;
using ShanDian.LSRestaurant.ViewModel.Dishes;
using ShanDian.SDK.Framework.DB;
using ShanDian.SDK.Framework.Model;

namespace ShanDian.LSRestaurant.Services
{
    class DishesServices : IDishesServices
    {
        private IRepositoryContext _repositoryContext;

        public DishesServices()
        {
            _repositoryContext = Global.RepositoryContext();
        }

        #region 点菜

        //[Get("menu/dishes/page", "获取点菜的菜品分页列表")]
        public SDApiResponse<PageListDto<DishMenuPageDto>> GetMenuDishesPage(DishQueryDto queryDto)
        {
            SDApiResponse<PageListDto<DishMenuPageDto>> apiResponse = new SDApiResponse<PageListDto<DishMenuPageDto>>();
            
            try
            {
                PageListDto<DishMenuPageDto> pageListDto = null;
                List<Dish> dishList = null;
                long.TryParse(queryDto.CategoryId, out var categoryId);
                if (categoryId > 0)//查询全部菜品（分类显示下的全部菜品：按分类排序->菜品排序）
                {
                    DishQuery dishQuery = new DishQuery()
                    {
                        CategoryId = categoryId,
                        DishType = new[] { (int)DishTypeEnum.CommDish, (int)DishTypeEnum.FixedSetMeal },
                        Keyword = queryDto.Keyword
                    };
                    dishList = GetDishesByCategoryIdPrivate(dishQuery).Where(t => t.IsOnline == 1).OrderBy(t => t.Sort).ToList();

                }
                else
                {
                    dishList = GetAllDishesPrivate(new[] { (int)DishTypeEnum.CommDish, (int)DishTypeEnum.FixedSetMeal }, queryDto.Keyword).Where(t => t.IsOnline == 1).ToList();
                }
                if (dishList != null && dishList.Count > 0)
                {
                    queryDto.PageIndex = queryDto.PageIndex < 1 ? 1 : queryDto.PageIndex;
                    var tempDishList = dishList.Skip((queryDto.PageIndex - 1) * queryDto.PageSize)
                        .Take(queryDto.PageSize);
                    var dishPageList = DishManageFactory.GetDishMenuPageDtoList(tempDishList.ToList());
                    var dishEstimateList = GetEstimates();
                    dishPageList.ForEach(t =>
                    {
                        var dishEstimate = dishEstimateList.Find(g => g.DishId.ToString() == t.Id);
                        if (dishEstimate != null)
                        {
                            t.EstimateCnt = dishEstimate.Amount;
                        }
                    });
                    pageListDto = new PageListDto<DishMenuPageDto>(dishPageList, queryDto.PageIndex, queryDto.PageSize, dishList.Count);
                    apiResponse.Data = pageListDto;
                }
                apiResponse.Code = 0;
            }
            catch (Exception ex)
            {
                //Flag = false;
                apiResponse.Code = DishesErrorCode.Code.ExecError;
                apiResponse.Msg = "获取点菜的菜品分页列表异常";
                //DishesLogUtility.Writer.SendFullError(ex);
            }
            return apiResponse;
        }

        //[Get("menu/dish/id", "获取点菜的菜品详情")]
        //public SDApiResponse<DishMenuDetailDto> GetMenuDishById(string id)
        //{ }

        ////[Get("menu/childDish/practices/id", "获取菜品做法")]
        //public SDApiResponse<List<PracticeDetailDto>> GetMenuChildDishPracticesById(string id)
        //{ }

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

        #region 估清

        #region 数据库管理

        public bool AlterEstimateBatch(List<Estimate> estimateList)
        {
            if (estimateList == null || estimateList.Count < 1)
            {
                return false;
            }
            bool res;
            try
            {
                var stringBuilder = new StringBuilder("BEGIN; ");
                foreach (var estimate in estimateList)
                {
                    stringBuilder.AppendFormat("{0} where id={1} ;", SqlFactory.GetUpdate(estimate), estimate.Id);
                    //stringBuilder.AppendFormat(@"update tb_estimate set dishId={1},amount={2},createTime='{3}',creator='{4}',modificationTime='{5}',modifitor='{6}' where id={0}; ",
                    //    estimate.Id, estimate.DishId, estimate.Amount, estimate.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"), estimate.Creator,
                    //    estimate.ModificationTime.ToString("yyyy-MM-dd HH:mm:ss"), estimate.Modifitor);
                }
                stringBuilder.Append("COMMIT;");
                _repositoryContext.Execute(stringBuilder.ToString(), null);

                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                //DishesLogUtility.Writer.SendFullError(ex);
            }
            return res;
        }

        /// <summary>
        /// 获取菜品估清
        /// </summary>
        /// <returns></returns>
        public List<Estimate> GetEstimates(long[] ids = null, long[] dishIds = null)
        {
            List<Estimate> dishEstimateList = new List<Estimate>();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("select id,dishId,amount,createTime,creator,modificationTime,modifitor from tb_estimate where 1=1 ");

                if (ids != null && ids.Length > 0)
                {
                    string idsStr = string.Join(",", ids);
                    sql.AppendFormat(" and id in ({0}) ", idsStr);
                }
                if (dishIds != null && dishIds.Length > 0)
                {
                    string dishIdsStr = string.Join(",", dishIds);
                    sql.AppendFormat(" and dishId in ({0}) ", dishIdsStr);
                }
                dishEstimateList = _repositoryContext.GetSet<Estimate>(sql.ToString());
            }
            catch (Exception ex)
            {
                //DishesLogUtility.Writer.SendFullError(ex);
            }
            return dishEstimateList;
        }

        #endregion

        #endregion

        #region 菜品管理

        #region 数据库管理

        /// <summary>
        /// 获取显示（isHidden = 0）的分类下的菜品
        /// </summary>
        /// <param name="dishType"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public List<Dish> GetAllDishesPrivate(int[] dishType = null, string keyword = "")
        {
            List<Dish> dishList = new List<Dish>();
            try
            {
                var colList = SqlFactory.GetTableColumn<Dish>("a");
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"select {0} from tb_dish a 
                    LEFT JOIN tb_categorydish b on a.categoryId = b.id
                    where b.isHidden = 0 and a.categoryId > 0 ", string.Join(",", colList));

                if (dishType != null && dishType.Length > 0)//菜品类型
                {
                    string dishTypeStr = string.Join(",", dishType);
                    sql.AppendFormat(" and a.dishType in ({0}) ", dishTypeStr);
                }
                if (!string.IsNullOrWhiteSpace(keyword))//模糊搜索
                {
                    sql.AppendFormat(" and (a.name like '%{0}%' or a.code like '%{0}%' or a.pinYin like '%{1}%') ", keyword.Replace("'", "''"), keyword.Replace("'", "''").ToUpper());
                }
                sql.AppendFormat(" ORDER BY b.sort,a.sort ;");
                string sqlStr = sql.ToString();
                dishList = _repositoryContext.GetSet<Dish>(sqlStr);
            }
            catch (Exception ex)
            {
                //DishesLogUtility.Writer.SendFullError(ex);
            }
            return dishList;
        }

        /// <summary>
        /// 获取菜品
        /// </summary>
        /// <param name="dishQuery"></param>
        /// <returns></returns>
        public List<Dish> GetDishesByCategoryIdPrivate(DishQuery dishQuery)
        {
            if (dishQuery == null)
            {
                return null;
            }
            List<Dish> dishList = new List<Dish>();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat(@"{0} where 1=1 ", SqlFactory.GetSelect<Dish>());

                #region 菜品搜索条件

                if (dishQuery.CategoryId > 0)//菜品分类
                {
                    sql.AppendFormat(" and categoryId={0} ", dishQuery.CategoryId);
                }

                if (dishQuery.DishType != null && dishQuery.DishType.Length > 0)//菜品类型
                {
                    string dishTypeStr = string.Join(",", dishQuery.DishType);
                    sql.AppendFormat(" and dishType in ({0}) ", dishTypeStr);
                }

                if (dishQuery.DishId > 0)//菜品Id
                {
                    sql.AppendFormat(" and id={0} ", dishQuery.DishId);
                }

                if (!string.IsNullOrWhiteSpace(dishQuery.DishName))//菜品名称
                {
                    sql.AppendFormat(" and name='{0}' ", dishQuery.DishName.Replace("'", "''"));
                }

                if (!string.IsNullOrWhiteSpace(dishQuery.Keyword))//模糊搜索
                {
                    sql.AppendFormat(" and (name like '%{0}%' or code like '%{0}%' or pinYin like '%{1}%') ", dishQuery.Keyword.Replace("'", "''"), dishQuery.Keyword.Replace("'", "''").ToUpper());
                }

                #endregion

                string sqpStr = sql.ToString();
                dishList = _repositoryContext.GetSet<Dish>(sqpStr);
            }
            catch (Exception ex)
            {
                //DishesLogUtility.Writer.SendFullError(ex);
            }
            return dishList;
        }

        #endregion

        #endregion
    }
}
