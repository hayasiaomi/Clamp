using ShanDian.AddIns;
using ShanDian.Machine.Model;
using ShanDian.Machine.Dto.ShanDianView;
using ShanDian.Machine.Services;
using ShanDian.Machine.Services.Data;
using ShanDian.Machine.Services.Factory;
using ShanDian.SDK.Framework;
using ShanDian.SDK.Framework.Model;
using ShanDian.Webwork;
using ShanDian.Webwork.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.Machine.Modules
{
    public class RoleModule : WebworkModule
    {
        private readonly IRoleService roleServices;

        public RoleModule() : base("Account")
        {
            this.roleServices = AddInManager.GetEntityService<IRoleService>("RoleService");
        }

        [Get("shop/permissions")]
        public dynamic GetPermissionsConfigure()
        {
            SDResponse<List<SimplePermissionDto>> response = new SDResponse<List<SimplePermissionDto>>();

            SDApiResponse<List<CPermissionCategoryDto>> apiResponse = this.roleServices.GetPermissionsConfigure();

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = UserFactory.GetSimplePermissionCategoryList(apiResponse.Data); ;
                    response.Flag = true;
                    response.Code = 200;
                    response.Message = apiResponse.Msg;
                }
                else
                {
                    response.Flag = false;
                    response.Code = AccountError.Code.ServerError;
                    response.Message = "善点API返回的数据格式不正确";
                }
            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response.SerializeObject();
        }

        [Get("roles")]
        public dynamic GetRoleSummary()
        {
            SDResponse<List<RoleSummaryDto>> response = new SDResponse<List<RoleSummaryDto>>();

            SDApiResponse<List<CRoleSummaryDto>> apiResponse = this.roleServices.GetRoleSummary();

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = UserFactory.GetRoleSummaryDtoList(apiResponse.Data);
                    response.Flag = true;
                    response.Code = 200;
                    response.Message = apiResponse.Msg;
                }
                else
                {
                    response.Flag = false;
                    response.Code = AccountError.Code.ServerError;
                    response.Message = "善点API返回的数据格式不正确";
                }
            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response.SerializeObject();
        }


        [Get("roles/roleId/permissions")]
        public dynamic GetRolePermissionsCategory(int roleId)
        {
            SDResponse<List<SimplePermissionDto>> response = new SDResponse<List<SimplePermissionDto>>();

            SDApiResponse<List<CPermissionCategoryDto>> apiResponse = this.roleServices.GetRolePermissionsCategory(roleId);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = UserFactory.GetSimplePermissionCategoryList(apiResponse.Data);
                    response.Flag = true;
                    response.Code = 200;
                    response.Message = apiResponse.Msg;
                }
                else
                {
                    response.Flag = false;
                    response.Code = AccountError.Code.ServerError;
                    response.Message = "善点API返回的数据格式不正确";
                }
            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response.SerializeObject();
        }

        [Post("roles")]
        public dynamic AddRolePermission(RolePermissionInDto rolePermissionInDto)
        {

            SDResponse<RoleSummaryDto> response = new SDResponse<RoleSummaryDto>();

            if (string.IsNullOrWhiteSpace(rolePermissionInDto?.RoleName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";

                return response;
            }

            SDApiResponse<CRoleSummaryDto> apiResponse = this.roleServices.AddRolePermission(rolePermissionInDto);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = new RoleSummaryDto { RoleId = apiResponse.Data.Id };
                    response.Flag = true;
                    response.Code = 200;
                    response.Message = apiResponse.Msg;
                }
                else
                {
                    response.Flag = false;
                    response.Code = AccountError.Code.ServerError;
                    response.Message = "善点API返回的数据格式不正确";
                }
            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response.SerializeObject();
        }

        [Put("roles/roleId")]
        public dynamic AlterRolePermission(RolePermissionInDto rolePermissionInDto)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(rolePermissionInDto?.OperatorName) || string.IsNullOrWhiteSpace(rolePermissionInDto.RoleName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";
                return response;
            }

            SDApiResponse<object> apiResponse = this.roleServices.AlterRolePermission(rolePermissionInDto);

            if (apiResponse.IsSuccess())
            {
                response.Result = apiResponse.Data;
                response.Flag = true;
                response.Code = 200;
                response.Message = apiResponse.Msg;
            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response.SerializeObject();
        }


        [Put("roles/roleId/users")]
        public dynamic AlterRoleUsers(UseArray useArray)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(useArray?.OperatorName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";
                return response;
            }

            SDApiResponse<object> apiResponse = this.roleServices.AlterRoleUsers(useArray);

            if (apiResponse.IsSuccess())
            {
                response.Result = apiResponse.Data;
                response.Flag = true;
                response.Code = 200;
                response.Message = apiResponse.Msg;
            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response.SerializeObject();
        }


        [Delete("roles/roleId")]
        public dynamic DeleteRole(int roleId, int operatorId, string operatorName)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(operatorName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";
                return response;
            }

            SDApiResponse<object> apiResponse = this.roleServices.DeleteRole(roleId, operatorId, operatorName);

            if (apiResponse.IsSuccess())
            {
                response.Result = apiResponse.Data;
                response.Flag = true;
                response.Code = 200;
                response.Message = apiResponse.Msg;
            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response.SerializeObject();
        }

    }
}
