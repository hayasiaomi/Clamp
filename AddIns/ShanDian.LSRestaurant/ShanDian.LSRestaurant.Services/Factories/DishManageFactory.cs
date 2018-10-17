using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ShanDian.Common.Extensions;
using ShanDian.LSRestaurant.Com;
using ShanDian.LSRestaurant.Interface.Dto.Dishes;
using ShanDian.LSRestaurant.Model.Dishes;
using ShanDian.LSRestaurant.ViewModel.Dishes;

namespace ShanDian.LSRestaurant.Factories
{
    public static class DishManageFactory
    {
        public static string GetEnumDescription(Enum enumValue)
        {
            string str = enumValue.ToString();
            FieldInfo field = enumValue.GetType().GetField(str);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (objs.Length == 0)
            {
                return string.Empty;
                //return str;
            }
            DescriptionAttribute da = (DescriptionAttribute)objs[0];
            return da.Description;
        }

        #region 菜品分类

        #region 本地菜品分类 To 菜品分类列表Dto

        public static List<CategoryDishSummaryDto> GetCategoryDishSummaryDtoList(List<CategoryDish> modelList)
        {
            List<CategoryDishSummaryDto> dtoList = new List<CategoryDishSummaryDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    var dto = GetCategoryDishSummaryDto(model);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static CategoryDishSummaryDto GetCategoryDishSummaryDto(CategoryDish model)
        {
            CategoryDishSummaryDto dto = null;
            if (model != null)
            {
                dto = new CategoryDishSummaryDto();
                dto.Id = model.Id.ToString();
                dto.Name = model.Name;
                dto.Sort = model.Sort;
                dto.IsHidden = model.IsHidden;
                dto.IsCharging = model.IsCharging;
                dto.IsPractice = model.IsPractice;
            }
            return dto;
        }

        #endregion

        #region 本地菜品分类 To 菜品分类列表Dto

        public static List<CateDishSimlpeDto> GetCateDishSimlpeDtoList(List<CategoryDish> modelList)
        {
            List<CateDishSimlpeDto> dtoList = new List<CateDishSimlpeDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    var dto = GetCateDishSimlpeDto(model);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(GetCateDishSimlpeDto(model));
                }
            }
            return dtoList;
        }

        public static CateDishSimlpeDto GetCateDishSimlpeDto(CategoryDish model)
        {
            CateDishSimlpeDto dto = null;
            if (model != null)
            {
                dto = new CateDishSimlpeDto();
                dto.Id = model.Id.ToString();
                dto.Name = model.Name;
                dto.Sort = model.Sort;
                dto.IsHidden = model.IsHidden;
            }
            return dto;
        }

        #endregion

        #endregion

        #region 加料辅菜

        #region 本地加料辅菜 To Dto

        public static List<ChargingFoodDto> GetChargingFoodDtoList(List<Dish> modelList)
        {
            List<ChargingFoodDto> dtoList = new List<ChargingFoodDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    var dto = GetChargingFoodDto(model);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static ChargingFoodDto GetChargingFoodDto(Dish model)
        {
            ChargingFoodDto dto = null;
            if (model != null)
            {
                dto = new ChargingFoodDto
                {
                    Id = model.Id.ToString(),
                    Name = model.Name,
                    Price = (decimal)model.Price / 100,
                    Unit = model.Unit,
                    Sort = model.Sort
                };
            }
            return dto;
        }

        #endregion

        #endregion

        #region 做法组

        #region 菜品分类：本地做法信息 To 做法Dto

        public static List<PracticeSummaryDto> GetPracticeSummaryDtoList(List<PracticeDetail> modelList)
        {
            List<PracticeSummaryDto> dtoList = new List<PracticeSummaryDto>();
            if (modelList != null && modelList.Count > 0)
            {

                var practiceDetailList = modelList.GroupBy(t => t.Id).ToList();
                foreach (var item in practiceDetailList)
                {
                    var practiceDetail = item.FirstOrDefault();
                    PracticeSummaryDto dto = new PracticeSummaryDto()
                    {
                        Id = item.Key.ToString(),
                        Name = practiceDetail?.Name,
                        Sort = practiceDetail?.Sort ?? 0,
                        PracticeInfoList = new List<PracticeInfoDto>()

                    };
                    foreach (var practiceInfo in item)
                    {
                        PracticeInfoDto practiceInfoDto = new PracticeInfoDto()
                        {
                            Id = practiceInfo.PracticeInfoId.ToString(),
                            Name = practiceInfo.PracticeInfoName,
                            Price = (decimal)practiceInfo.Price / 100,
                            Sort = practiceInfo.PracticeInfoSort
                        };
                        dto.PracticeInfoList.Add(practiceInfoDto);
                    }
                    //dto.PracticeContent = item.OrderBy(t => t.PracticeInfoSort).Select(t => t.PracticeInfoName).ToJson();
                    //dto.PracticeContent = dto.PracticeContent.TrimStart('[').TrimEnd(']').Replace("\"","").Replace(",","、");
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }
        #endregion

        /// <summary>
        /// 本地做法组、详情 To 做法Dto
        /// </summary>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public static PracticeDetailDto GetPracticeDetailDto(List<PracticeDetail> modelList)
        {
            PracticeDetailDto dto = new PracticeDetailDto();
            if (modelList != null && modelList.Count > 0)
            {
                dto.Id = modelList.FirstOrDefault().Id.ToString();
                dto.Name = modelList.FirstOrDefault()?.Name;
                dto.Sort = modelList.FirstOrDefault().Sort;
                dto.PracticeInfoList = new List<PracticeInfoDto>();
                foreach (var model in modelList)
                {
                    PracticeInfoDto practiceInfoDto = new PracticeInfoDto()
                    {
                        Id = model.PracticeInfoId.ToString(),
                        Name = model.PracticeInfoName,
                        Price = ((decimal)model.Price) / 100,
                        Sort = model.PracticeInfoSort,
                    };
                    dto.PracticeInfoList.Add(practiceInfoDto);
                }
            }
            return dto;
        }

        #region 做法详情Dto To 本地model

        public static List<PracticeInfo> GetPracticeInfoList(List<PracticeInfoDto> dtoList, long id)
        {
            List<PracticeInfo> modelList = new List<PracticeInfo>();
            if (dtoList != null && dtoList.Count > 0)
            {
                foreach (var dto in dtoList)
                {
                    var model = GetPracticeInfo(dto, id);
                    if (model == null)
                    {
                        continue;
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }

        public static PracticeInfo GetPracticeInfo(PracticeInfoDto dto, long id)
        {
            PracticeInfo model = null;
            if (dto != null)
            {
                model = new PracticeInfo
                {
                    PracticeGroupId = id,
                    Name = dto.Name,
                    Price = (int)(dto.Price * 100),
                    Sort = dto.Sort,
                    CreateTime = DateTime.Now
                };
                //model.Creator =;
                //model.ModificationTime =;
                //model.Modifitor;
            }
            return model;
        }

        #endregion

        #endregion


        #region 菜品管理

        public static List<DishSummaryDto> GetDishSummaryDtoList(List<Dish> modelList)
        {
            List<DishSummaryDto> dtoList = new List<DishSummaryDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    var dto = GetDishSummaryDto(model);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static DishSummaryDto GetDishSummaryDto(Dish model)
        {
            DishSummaryDto dto = null;
            if (model != null)
            {
                dto = new DishSummaryDto
                {
                    Id = model.Id.ToString(),
                    Code = model.Code,
                    Name = model.Name,
                    IsOnline = model.IsOnline,
                    Price = ((decimal)model.Price) / 100,
                    Sort = model.Sort,
                    DishType = model.DishType
                };
            }
            return dto;

        }
        //dishSkusDetail

        public static List<DishSimpleDto> GetDishSimpleDtoList(List<DishSkusDetail> modelList)
        {
            List<DishSimpleDto> dtoList = new List<DishSimpleDto>();
            if (modelList != null && modelList.Count > 0)
            {
                int sort = 1;
                foreach (var model in modelList)
                {
                    var dto = GetDishSimpleDto(model);
                    if (dto == null)
                    {
                        continue;
                    }
                    dto.Sort = sort;
                    dtoList.Add(dto);
                    sort++;
                }
            }
            return dtoList;
        }

        public static DishSimpleDto GetDishSimpleDto(DishSkusDetail model)
        {
            DishSimpleDto dto = null;
            if (model != null)
            {
                dto = new DishSimpleDto
                {
                    Id = model.Id.ToString(),
                    Name = model.Name,
                    Price = ((decimal)model.Price) / 100,
                    Unit = model.Unit,
                    //Sort = model.Sort
                };
                if (model.IsSkus == 1 && model.SkusId > 0)
                {
                    dto.Price = ((decimal)model.SkusPrice) / 100;
                    dto.Unit = model.SkusName;
                    //dto.Sort = model.SkusSort;
                }
            }
            return dto;
        }

        public static List<DishSimpleDto> GetDishSimpleDtoList(List<Dish> modelList)
        {
            List<DishSimpleDto> dtoList = new List<DishSimpleDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    var dto = GetDishSimpleDto(model);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static DishSimpleDto GetDishSimpleDto(Dish model)
        {
            DishSimpleDto dto = null;
            if (model != null)
            {
                dto = new DishSimpleDto
                {
                    Id = model.Id.ToString(),
                    Name = model.Name,
                    Price = ((decimal)model.Price) / 100,
                    Sort = model.Sort
                };
            }
            return dto;
        }

        #region 份量dto To 本地model

        public static List<SkusDish> GetSkusDishList(List<SkusDishDto> dtoList)
        {
            List<SkusDish> modelList = null;
            if (dtoList != null && dtoList.Count > 0)
            {
                modelList = new List<SkusDish>();
                foreach (var dto in dtoList)
                {
                    var model = GetSkusDish(dto);
                    if (model == null)
                    {
                        continue;
                    }
                    modelList.Add(model);
                }
            }
            return modelList;
        }

        public static SkusDish GetSkusDish(SkusDishDto dto)
        {
            SkusDish model = null;
            if (dto != null)
            {
                model = new SkusDish
                {
                    //DishId = dto.,
                    Name = dto.Name,
                    Price = (int)(dto.Price * 100),
                    Sort = dto.Sort,
                    //CreateTime =,
                    //Creator =,
                    //ModificationTime =,
                    //Modifitor
                };
            }
            return model;
        }

        #endregion

        public static List<PracticeDish> GetPracticeDishList(List<PracticeSimpleDto> practiceList)
        {
            List<PracticeDish> practiceDishList = new List<PracticeDish>(); ;
            if (practiceList != null && practiceList.Count > 0)
            {
                foreach (var practice in practiceList)
                {
                    long practiceGroupId;
                    long.TryParse(practice.Id, out practiceGroupId);
                    PracticeDish practiceDish = new PracticeDish()
                    {
                        PracticeGroupId = practiceGroupId,
                        Sort = practice.Sort
                    };
                    Dictionary<string, int> dishRuleDic = new Dictionary<string, int>();
                    dishRuleDic.Add(DishRuleEnum.IsMust.ToString(), practice.IsMust);
                    practiceDish.DishRule = dishRuleDic.ToJson();
                    practiceDishList.Add(practiceDish);
                }
            }
            return practiceDishList;
        }

        public static List<DishGroup> GetDishGroupList(List<DishGroupDto> dishGroupDtoList, out List<ChildDish> childDishList, int groupType)
        {
            List<DishGroup> dishGroupList = new List<DishGroup>();
            childDishList = new List<ChildDish>();
            if (dishGroupDtoList != null && dishGroupDtoList.Count > 0)
            {
                foreach (var dishGroupDto in dishGroupDtoList)
                {
                    DishGroup dishGroup = new DishGroup()
                    {
                        //Name = "",
                        //DishId = dishId,
                        //CategoryId = categoryId,
                        GroupType = groupType,
                        //Sort=1,
                        //DishRule="",
                        CreateTime = DateTime.Now,
                        //Creator = categoryDish.Creator,
                        ModificationTime = DateTime.Now,
                        //Modifitor = categoryDish.Modifitor
                    };
                    dishGroupList.Add(dishGroup);
                    var modelList = GetChildDishList(dishGroupDto.ChildDishList);
                    if (modelList != null && modelList.Count > 0)
                    {
                        modelList.ForEach(t => t.DishGroupId = dishGroup.Id);
                        childDishList.AddRange(modelList);
                    }
                }
            }
            return dishGroupList;
        }

        public static List<ChildDish> GetChildDishList(List<ChildDishDto> dtoList)
        {
            List<ChildDish> childDishList = null;
            if (dtoList != null && dtoList.Count > 0)
            {
                childDishList = new List<ChildDish>();
                foreach (var dto in dtoList)
                {
                    var model = GetChildDish(dto);
                    if (model == null)
                    {
                        continue;
                    }
                    childDishList.Add(model);
                }
            }
            return childDishList;
        }

        public static ChildDish GetChildDish(ChildDishDto dto)
        {
            long.TryParse(dto.SkusId, out var skusId);
            ChildDish model = null;
            if (dto != null && long.TryParse(dto.DishId, out var dishId))
            {
                model = new ChildDish()
                {
                    //DishGroupId =,
                    DishId = dishId,
                    SkusId = skusId,
                    Amount = dto.Amount,
                    Sort = dto.Sort,
                    CreateTime = DateTime.Now,
                    //Creator =,
                    ModificationTime = DateTime.Now,
                    //Modifitor=
                };
            }
            return model;
        }

        #region 本地份量model To Dto

        public static List<SkusDishDto> GetSkusDishDtoList(List<SkusDish> modelList)
        {
            List<SkusDishDto> dtoList = new List<SkusDishDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    var dto = GetSkusDishDto(model);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static SkusDishDto GetSkusDishDto(SkusDish model)
        {
            SkusDishDto dto = null;
            if (model != null)
            {
                dto = new SkusDishDto();
                //dto.SkusId = model.SkusId;
                dto.Name = model.Name;
                dto.Price = ((decimal)model.Price) / 100;
                dto.Sort = model.Sort;
            }
            return dto;
        }

        #endregion

        #region 本地做法model To Dto

        public static List<PracticeDetailDto> GetPracticeDetailDtoList(List<DishPracticeDetail> modelList)
        {
            List<PracticeDetailDto> dtoList = new List<PracticeDetailDto>();
            if (modelList != null && modelList.Count > 0)
            {
                var practiceGroupList = modelList.GroupBy(t => t.PracticeGroupId);
                foreach (var item in practiceGroupList)
                {
                    var dto = GetPracticeDetailDto(item);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static PracticeDetailDto GetPracticeDetailDto(IGrouping<long, DishPracticeDetail> modelList)
        {
            PracticeDetailDto dto = null;
            if (modelList != null && modelList.Any())
            {
                var model = modelList.FirstOrDefault();
                dto = new PracticeDetailDto()
                {
                    Id = modelList.Key.ToString(),
                    Name = model?.PracticeGroupName,
                    Sort = model?.Sort ?? 0,
                    PracticeInfoList = new List<PracticeInfoDto>()
                };
                Dictionary<string, int> dishRuleDic = model?.DishRule.ToDeserialize<Dictionary<string, int>>();
                if (dishRuleDic != null && dishRuleDic.ContainsKey(DishRuleEnum.IsMust.ToString()))
                {
                    dto.IsMust = dishRuleDic[DishRuleEnum.IsMust.ToString()];
                }
                foreach (var practiceGroup in modelList)
                {
                    PracticeInfoDto practiceInfoDto = new PracticeInfoDto()
                    {
                        Id = practiceGroup.PracticeInfoId.ToString(),
                        Name = practiceGroup.PracticeInfoName,
                        Price = (decimal)practiceGroup.Price / 100,
                        Sort = practiceGroup.PracticeInfoSort
                    };
                    dto.PracticeInfoList.Add(practiceInfoDto);
                }
            }
            return dto;
        }

        #endregion

        #region 本地子菜品model To Dto

        public static List<DishGroupSummaryDto> GetDishGroupSummaryDtoList(List<ChildDishDetail> modelList)
        {
            List<DishGroupSummaryDto> dtoList = new List<DishGroupSummaryDto>();
            if (modelList != null && modelList.Count > 0)
            {
                var dishGroupList = modelList.GroupBy(t => t.Id);
                foreach (var item in dishGroupList)
                {
                    var dto = GetDishGroupSummaryDto(item);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static DishGroupSummaryDto GetDishGroupSummaryDto(IGrouping<long, ChildDishDetail> modelList)
        {
            DishGroupSummaryDto dto = null;
            if (modelList != null && modelList.Any())
            {
                var model = modelList.FirstOrDefault();
                dto = new DishGroupSummaryDto()
                {
                    Id = modelList.Key.ToString(),
                    GroupType = model?.GroupType ?? 40,
                    ChildDishList = new List<ChildDishSummaryDto>()
                };
                foreach (var dishGroup in modelList)
                {
                    ChildDishSummaryDto childDishDto = new ChildDishSummaryDto()
                    {
                        Id = dishGroup.ChildDishId.ToString(),
                        CategoryId = dishGroup.CategoryId.ToString(),
                        DishId = dishGroup.DishId.ToString(),
                        DishName = dishGroup.Name,
                        SkusId = dishGroup.SkusId.ToString(),
                        Unit = dishGroup.SkusId > 0 ? dishGroup.SkusName : dishGroup.Unit,
                        Price = (decimal)(dishGroup.SkusId > 0 ? dishGroup.SkusPrice : dishGroup.Price) / 100,
                        Amount = dishGroup.Amount,
                        Sort = dishGroup.Sort
                    };
                    dto.ChildDishList.Add(childDishDto);
                }
            }
            return dto;
        }

        #endregion

        public static DishDetailDto GetDishDetailDto(Dish model)
        {
            DishDetailDto dto = null;
            if (model != null)
            {
                dto = new DishDetailDto()
                {
                    Id = model.Id.ToString(),
                    Code = model.Code,
                    Name = model.Name,
                    PinYin = model.PinYin,
                    Price = ((decimal)model.Price) / 100,
                    DishType = model.DishType,
                    CategoryId = model.CategoryId.ToString(),
                    //CategoryDishName =,
                    Unit = model.Unit,
                    IsOnline = model.IsOnline,
                    IsDiscount = model.IsDiscount,
                    BoxFee = (decimal)model.BoxFee / 100,
                    IsSkus = model.IsSkus,
                    IsPractice = model.IsPractice,
                    IsCharging = model.IsCharging
                };
            }
            return dto;
        }


        #endregion

        #region 点菜

        #region 本地菜品列表model To 点菜列表dto

        public static List<DishMenuPageDto> GetDishMenuPageDtoList(List<Dish> modelList)
        {
            List<DishMenuPageDto> dtoList = new List<DishMenuPageDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    var dto = GetDishMenuPageDto(model);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static DishMenuPageDto GetDishMenuPageDto(Dish model)
        {
            DishMenuPageDto dto = null;
            if (model != null)
            {
                dto = new DishMenuPageDto()
                {
                    Id = model.Id.ToString(),
                    Code = model.Code,
                    Name = model.Name,
                    Price = (decimal)model.Price / 100,
                    Sort = model.Sort
                };
            }
            return dto;
        }

        #endregion

        public static DishMenuDetailDto GetDishMenuDetailDto(Dish model)
        {
            DishMenuDetailDto dto = null;
            if (model != null)
            {
                dto = new DishMenuDetailDto()
                {
                    Id = model.Id.ToString(),
                    Code = model.Code,
                    Name = model.Name,
                    Price = (decimal)model.Price / 100,
                    DishType = model.DishType,
                    Unit = model.Unit,
                    IsDiscount = model.IsDiscount,
                    BoxFee = (decimal)model.BoxFee / 100
                };
            }
            return dto;
        }

        public static List<MenuDishGroupDto> GetMenuDishGroupDtoList(List<ChildDishDetail> modelList)
        {
            List<MenuDishGroupDto> dtoList = new List<MenuDishGroupDto>();
            if (modelList != null && modelList.Count > 0)
            {
                var dishGroupList = modelList.GroupBy(t => t.Id);
                foreach (var item in dishGroupList)
                {
                    var dto = GetMenuDishGroupDto(item);
                    if (dto == null)
                    {
                        continue;
                    }
                    dtoList.Add(dto);
                }
            }
            return dtoList;
        }

        public static MenuDishGroupDto GetMenuDishGroupDto(IGrouping<long, ChildDishDetail> modelList)
        {
            MenuDishGroupDto dto = null;
            if (modelList != null && modelList.Any())
            {
                var model = modelList.FirstOrDefault();
                dto = new MenuDishGroupDto()
                {
                    Id = modelList.Key.ToString(),
                    GroupType = model?.GroupType ?? 40,
                    ChildDishList = new List<MenuChildDishDto>()
                };
                foreach (var dishGroup in modelList)
                {
                    MenuChildDishDto childDishDto = new MenuChildDishDto()
                    {
                        Id = dishGroup.ChildDishId.ToString(),
                        DishId = dishGroup.DishId.ToString(),
                        Name = dishGroup.Name,
                        IsOnline = dishGroup.IsOnline,
                        Price = (decimal)(dishGroup.SkusId > 0 ? dishGroup.SkusPrice : dishGroup.Price) / 100,
                        Unit = dishGroup.SkusId > 0 ? dishGroup.SkusName : dishGroup.Unit,
                        Amount = dishGroup.Amount,
                        DishType = dishGroup.DishType,
                        Sort = dishGroup.Sort,
                        IsPractice = dishGroup.IsPractice,
                        IsCharging = dishGroup.IsCharging
                    };
                    dto.ChildDishList.Add(childDishDto);
                }
            }
            return dto;
        }

        #endregion


    }
}
