using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.Machine.Model;
using ShanDian.Machine.Dto.ShanDianView;

namespace ShanDian.Machine.Services.Factory
{
    public class UserFactory
    {
        /// <summary>
        /// 善点云登录数据model转本地model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static LoginDto GetLoginDto(CLoginOutDto model)
        {
            LoginDto dto = new LoginDto();
            if (model != null)
            {
                dto.UserId = model.Id;
                dto.UserName = model.NickName;
                dto.UserCode = model.UserName;
                dto.Pwd = model.Pwd;
                dto.Mobile = model.Mobile;
                dto.Sex = model.Sex;
                dto.IsAdmin = model.IsDefaultAdmin;
                dto.IsFirstLogin = model.IsFirstLogin;
                dto.RoleName = model.RoleName;
                dto.Permissions = GetPermissionCategoryList(model.Permissions);
            }
            return dto;
        }

        #region 善点云权限列表model转本地model

        public static List<PermissionDto> GetPermissionCategoryList(List<CPermissionCategoryDto> modelList)
        {
            List<PermissionDto> dto = new List<PermissionDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (CPermissionCategoryDto model in modelList)
                {
                    var permissionDto = GetPermissionDto(model);
                    dto.Add(permissionDto);
                    var permissionDtoList = GetPermissionDtoList(model, permissionDto.Code);
                    //permissionDtoList.ForEach(t => t.KindCode = permissionDto.Code);
                    //permissionDtoList.ForEach(t => t.CategoryCode = permissionDto.Code);
                    dto.AddRange(permissionDtoList.Where(t => t.Code != permissionDto.Code));

                }
            }
            return dto;
        }

        public static List<PermissionDto> GetPermissionDtoList(CPermissionCategoryDto model, string KindCode)
        {
            List<PermissionDto> dtoList = new List<PermissionDto>();
            if (model != null)
            {
                var permissionDto = GetPermissionDto(model);
                permissionDto.KindCode = KindCode;
                dtoList.Add(permissionDto);
                foreach (var item in model.Data)
                {
                    var itemList = GetPermissionDtoList(item, KindCode);
                    itemList.ForEach(t => t.CategoryCode = model.Code);
                    dtoList.AddRange(itemList);
                }
            }
            return dtoList;
        }

        //public static List<PermissionDto> GetPermissionDtoList(List<CPermissionDto> modelList)
        //{
        //    List<PermissionDto> dtoList = new List<PermissionDto>();
        //    if (modelList != null && modelList.Count > 0)
        //    {
        //        foreach (var model in modelList)
        //        {
        //            dtoList.Add(GetPermissionDto(model));
        //        }
        //    }
        //    return dtoList;
        //}
        public static PermissionDto GetPermissionDto(CPermissionDto model)
        {
            PermissionDto dto = new PermissionDto();
            if (model != null)
            {
                dto.Name = model.Name;
                dto.Code = model.Code;
                dto.Url = model.Url;
                dto.Sort = model.Sort;
                dto.Icon = model.Icon;
                dto.IsInner = model.IsInner;
                dto.Token = model.Token;
            }
            return dto;
        }

        #endregion

        #region 善点云权限基本信息model转本地model


        public static List<SimplePermissionDto> GetSimplePermissionCategoryList(List<CPermissionCategoryDto> modelList)
        {
            List<SimplePermissionDto> dto = new List<SimplePermissionDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (CPermissionCategoryDto model in modelList)
                {
                    SimplePermissionDto simplePermissionDto = GetSimplePermissionDto(model);
                    dto.Add(simplePermissionDto);
                    var simplePermissionDtoList = GetSimplePermissionDtoList(model, simplePermissionDto.Code);

                    dto.AddRange(simplePermissionDtoList.Where(t => t.Code != simplePermissionDto.Code));

                }
            }
            return dto;
        }

        public static List<SimplePermissionDto> GetSimplePermissionDtoList(CPermissionCategoryDto model, string kindCode)
        {
            List<SimplePermissionDto> dtoList = new List<SimplePermissionDto>();
            if (model != null)
            {
                var simplePermissionDto = GetSimplePermissionDto(model);
                simplePermissionDto.KindCode = kindCode;
                dtoList.Add(simplePermissionDto);
                foreach (var item in model.Data)
                {
                    var itemList = GetSimplePermissionDtoList(item, kindCode);
                    itemList.ForEach(t => t.CategoryCode = model.Code);
                    dtoList.AddRange(itemList);
                }
            }
            return dtoList;
        }

        //public static List<SimplePermissionDto> GetSimplePermissionDtoList(List<CPermissionDto> modelList)
        //{
        //    List<SimplePermissionDto> dtoList = new List<SimplePermissionDto>();
        //    if (modelList != null && modelList.Count > 0)
        //    {
        //        foreach (var model in modelList)
        //        {
        //            dtoList.Add(GetSimplePermissionDto(model));
        //        }
        //    }
        //    return dtoList;
        //}
        public static SimplePermissionDto GetSimplePermissionDto(CPermissionDto model)
        {
            SimplePermissionDto dto = new SimplePermissionDto();
            if (model != null)
            {
                dto.Name = model.Name;
                dto.Code = model.Code;
                dto.Sort = model.Sort;
                dto.Icon = model.Icon;
                dto.IsMoreConfig = model.IsMoreConfig;
                dto.ConfigUrl = model.ConfigUrl;
            }
            return dto;
        }

        #endregion

        #region 善点云权限列表model转本地model

        public static List<UserSummaryDto> GetUserSummaryDtoList(List<CUserSummaryDto> modelList)
        {
            List<UserSummaryDto> dtoList = new List<UserSummaryDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    dtoList.Add(GetUserSummaryDto(model));
                }
            }
            return dtoList;
        }
        /// <summary>
        /// 善点云用户概述信息model转本地model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static UserSummaryDto GetUserSummaryDto(CUserSummaryDto model)
        {
            UserSummaryDto dto = new UserSummaryDto();
            if (model != null)
            {
                dto.UserId = model.Id;
                dto.UserName = model.NickName;
                dto.UserCode = model.UserName;
                dto.Status = model.Status;
                dto.IsAdmin = model.IsDefaultAdmin;
                dto.CreationTime = model.CreationTime;
                dto.RoleId = model.RoleId;
                dto.RoleName = model.RoleName;
            }
            return dto;
        }

        #endregion

        /// <summary>
        /// 善点云用户详情model转本地model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static UserDetailDto GetUserDetailDto(CUserDetailDto model)
        {
            UserDetailDto dto = new UserDetailDto();
            if (model != null)
            {
                dto.UserId = model.Id;
                dto.UserName = model.NickName;
                dto.UserCode = model.UserName;
                dto.Mobile = model.Mobile;
                dto.Sex = model.Sex;
                dto.Status = model.Status;
                dto.IsAdmin = model.IsDefaultAdmin;
                dto.RoleId = model.RoleId;
                dto.RoleName = model.RoleName;
                dto.Permissions = GetSimplePermissionCategoryList(model.Permissions);
            }
            return dto;
        }

        #region 善点云角色概述信息model转本地model

        public static List<RoleSummaryDto> GetRoleSummaryDtoList(List<CRoleSummaryDto> modelList)
        {
            List<RoleSummaryDto> dtoList = new List<RoleSummaryDto>();
            if (modelList != null && modelList.Count > 0)
            {
                foreach (var model in modelList)
                {
                    dtoList.Add(GetRoleSummaryDto(model));
                }
            }
            return dtoList;
        }
        public static RoleSummaryDto GetRoleSummaryDto(CRoleSummaryDto model)
        {
            RoleSummaryDto dto = new RoleSummaryDto();
            if (model != null)
            {
                dto.RoleId = model.Id;
                dto.RoleName = model.Name;
                dto.IsAdmin = model.IsAdmin;
                dto.UserCount = model.UserCount;
            }
            return dto;
        }

        #endregion




    }

}
