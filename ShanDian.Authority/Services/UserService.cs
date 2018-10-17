using ShanDian.Machine.Services;
using ShanDian.Machine.Services.Cloud;
using ShanDian.Machine.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using ShanDian.Machine.Model;
using ShanDian.Machine.Dto.ShanDianView;
using ShanDian.Machine.Model.LicenseCode;
using ShanDian.Machine.Services.Factory;
using ShanDian.SDK.Framework.Helpers;
using ShanDian.Common.HTTP;
using Newtonsoft.Json;
using ShanDian.SDK.Framework.Model;
using ShanDian.Machine.Dto;
using ShanDian.Software.SDK;

namespace ShanDian.Machine.Services
{
    public class UserService : IUserService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public SDApiResponse<CLoginOutDto> Login(string loginName, string pwd)
        {
            SDApiResponse<CLoginOutDto> apiResponse;

            try
            {
                var cLoginInDto = new CLoginInDto()
                {
                    LoginName = loginName,
                    Pwd = pwd
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest("/v1/users/login", HttpMethod.POST, JsonConvert.SerializeObject(cLoginInDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<CLoginOutDto>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<CLoginOutDto>();
                    apiResponse.Code = 10014;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<CLoginOutDto>();
                apiResponse.Code = 10014;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }

        ////注销
        //public void DeleteToken(string token)
        //{
        //    var jurisdictionServices = ServiceLocator.Instance.Resolve<IJurisdictionServices>();
        //    jurisdictionServices.DeleteToken(token);
        //    Flag = true;
        //    //return true;
        //}

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="keyword">姓名、账号模糊搜索</param>
        /// <returns></returns>
        public SDApiResponse<List<CUserSummaryDto>> GetUsers(string keyword)
        {
            SDApiResponse<List<CUserSummaryDto>> apiResponse;

            try
            {

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest("/v1/users?keyword=" + keyword, HttpMethod.GET, string.Empty);

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<List<CUserSummaryDto>>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<List<CUserSummaryDto>>();
                    apiResponse.Code = 10014;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }


            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<List<CUserSummaryDto>>();
                apiResponse.Code = 10014;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SDApiResponse<CUserDetailDto> GetUserDetail(int userId)
        {

            SDApiResponse<CUserDetailDto> apiResponse;

            try
            {
                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/users/{userId}", HttpMethod.GET, string.Empty);

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<CUserDetailDto>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<CUserDetailDto>();
                    apiResponse.Code = 10014;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }


            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<CUserDetailDto>();
                apiResponse.Code = 10014;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="userInDto"></param>
        public SDApiResponse<object> AddUser(UserInDto userInDto)
        {
            SDApiResponse<object> apiResponse;

            try
            {

                CUserInDto cUserInDto = new CUserInDto()
                {
                    UserName = userInDto.UserCode,
                    NickName = userInDto.UserName,
                    Mobile = userInDto.Mobile,
                    Sex = userInDto.Sex,
                    Pwd = userInDto.Pwd,
                    Status = userInDto.Status,
                    RoleId = userInDto.RoleId,
                    PermissionList = userInDto.PermissionList,
                    OperatorId = userInDto.OperatorId,
                    OperatorName = userInDto.OperatorName
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest("/v1/users", HttpMethod.POST, JsonConvert.SerializeObject(cUserInDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<object>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<object>();
                    apiResponse.Code = 10014;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<object>();
                apiResponse.Code = 10014;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="userInfoInDto"></param>
        public SDApiResponse<object> AlterUser(UserInfoInDto userInfoInDto)
        {
            SDApiResponse<object> apiResponse;

            try
            {

                CUserInfoInDto cUserInfoInVDto = new CUserInfoInDto()
                {
                    UserName = userInfoInDto.UserCode,
                    NickName = userInfoInDto.UserName,
                    Mobile = userInfoInDto.Mobile,
                    Sex = userInfoInDto.Sex,
                    Status = userInfoInDto.Status,
                    OperatorId = userInfoInDto.OperatorId,
                    OperatorName = userInfoInDto.OperatorName
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/users/{userInfoInDto.UserId}", HttpMethod.PUT, JsonConvert.SerializeObject(cUserInfoInVDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<object>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<object>();
                    apiResponse.Code = 10014;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<object>();
                apiResponse.Code = 10014;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="operatorId"></param>
        /// <param name="operatorName"></param>
        public SDApiResponse<object> DeleteUser(int userId, int operatorId, string operatorName)
        {
            SDApiResponse<object> apiResponse;

            try
            {

                InputDto inputDto = new InputDto()
                {
                    OperatorId = operatorId,
                    OperatorName = operatorName
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/users/{userId}", HttpMethod.DELETE, JsonConvert.SerializeObject(inputDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<object>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<object>();
                    apiResponse.Code = 10014;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<object>();
                apiResponse.Code = 10014;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }

        /// <summary>
        /// 获取指定权限用户列表
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public SDApiResponse<List<CSimpleUserDto>> GetUsersByPermissionCode(string code)
        {
            SDApiResponse<List<CSimpleUserDto>> apiResponse;

            try
            {
                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/permissions/users?code={code}", HttpMethod.GET, null);

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<List<CSimpleUserDto>>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<List<CSimpleUserDto>>();
                    apiResponse.Code = 10014;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }

            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<List<CSimpleUserDto>>();
                apiResponse.Code = AccountError.Code.ComponentError;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }
            return apiResponse;
        }


        /// <summary>
        /// 修改用户角色
        /// </summary>
        /// <param name="userRolePermissionInDto"></param>
        public SDApiResponse<object> AlterUserRole(UserRolePermissionInDto userRolePermissionInDto)
        {

            SDApiResponse<object> apiResponse;

            try
            {

                CUserRolePermissionInDto cUserRolePermissionInDto = new CUserRolePermissionInDto()
                {
                    RoleId = userRolePermissionInDto.RoleId,
                    PermissionList = userRolePermissionInDto.PermissionList,
                    OperatorId = userRolePermissionInDto.OperatorId,
                    OperatorName = userRolePermissionInDto.OperatorName
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/users/{userRolePermissionInDto.UserId}/pwd", HttpMethod.PUT, JsonConvert.SerializeObject(cUserRolePermissionInDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<object>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<object>();
                    apiResponse.Code = AccountError.Code.ComponentError;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<object>();
                apiResponse.Code = AccountError.Code.ComponentError;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }

        /// <summary>
        /// 验证账号是否可用
        /// </summary>
        /// <param name="verificationUserInDto"></param>
        public SDApiResponse<object> VerificationUser(VerificationUserInDto verificationUserInDto)
        {

            SDApiResponse<object> apiResponse;

            try
            {

                CVerificationUserInDto cVerificationUserInDto = new CVerificationUserInDto()
                {
                    UserName = verificationUserInDto.UserCode,
                    Mobile = verificationUserInDto.Mobile
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/permissions/verification", HttpMethod.POST, JsonConvert.SerializeObject(cVerificationUserInDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<object>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<object>();
                    apiResponse.Code = AccountError.Code.ComponentError;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<object>();
                apiResponse.Code = AccountError.Code.ComponentError;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }


        /// <summary>
        /// 使用账号、密码获取授权码
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="oldPwd">原始旧密码</param>
        /// <param name="operatorId">操作人id</param>
        /// <param name="operatorName">操作人</param>
        /// <returns>授权码</returns>
        public SDApiResponse<string> LicenseCodePwdByUserId(int userId, string oldPwd, int operatorId, string operatorName)
        {
            SDApiResponse<string> apiResponse;

            try
            {
                CVerificationPwdInDto cVerificationPwdInDto = new CVerificationPwdInDto()
                {
                    UserId = userId,
                    Pwd = oldPwd
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/permissions/verification", HttpMethod.POST, JsonConvert.SerializeObject(cVerificationPwdInDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<string>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<string>();
                    apiResponse.Code = AccountError.Code.ComponentError;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }
            }
            catch (Exception ex)
            {
                apiResponse = new SDApiResponse<string>();
                apiResponse.Code = AccountError.Code.ComponentError;
                apiResponse.Msg = "系统繁忙，请稍后再试";
                apiResponse.Ts = DateTime.Now.Ticks;
            }

            return apiResponse;
        }

        /// <summary>
        /// 创建重置密码的授权码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="operatorId"></param>
        /// <param name="operatorName"></param>
        /// <param name="permissionCode"></param>
        /// <returns></returns>
        public SDApiResponse<string> LicenseCodePwdByPermissionCode(int userId, int operatorId, string operatorName, string permissionCode)
        {
            SDApiResponse<string> apiResponse = new SDApiResponse<string>();

            try
            {
                SDApiResponse<List<CSimpleUserDto>> sdApiResponse = this.GetUsersByPermissionCode(permissionCode);

                apiResponse.Code = sdApiResponse.Code;
                apiResponse.Msg = sdApiResponse.Msg;

                if (sdApiResponse.IsSuccess())
                {
                    List<CSimpleUserDto> cSimpleUserDtoList = sdApiResponse.Data;

                    if (cSimpleUserDtoList == null || cSimpleUserDtoList.Count < 1 || !cSimpleUserDtoList.Exists(t => t.Id == operatorId))
                    {
                        apiResponse.Code = AccountError.Code.NotPermissionError;
                        apiResponse.Msg = "账号未授权";
                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse.Code = AccountError.Code.ComponentError;
                apiResponse.Msg = "系统繁忙，请稍后再试";
            }

            return apiResponse;
        }


        /// <summary>
        /// 使用授权码重置密码
        /// </summary>
        /// <param name="messageCode">授权码</param>
        /// <param name="password">新密码</param>
        public SDApiResponse<object> Password(int userId, int operatorId, string operatorName, string password)
        {
            SDApiResponse<object> apiResponse = new SDApiResponse<object>();
            try
            {
                CUserPwdInDto cUserPwdInDto = new CUserPwdInDto()
                {
                    Pwd = password,
                    OperatorId = operatorId,
                    OperatorName = operatorName
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/users/{userId}/pwd", HttpMethod.PUT, JsonConvert.SerializeObject(cUserPwdInDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<object>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<object>();
                    apiResponse.Code = AccountError.Code.ComponentError;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }

            }
            catch (Exception ex)
            {
                apiResponse.Code = AccountError.Code.ComponentError;
                apiResponse.Msg = "系统繁忙，请稍后再试";
            }

            return apiResponse;
        }

        /// <summary>
        /// 单次针对某个URL（router）进行授权（用户、密码验证授权）
        /// </summary>
        /// <param name="userId">请求获取一次权限用户Id</param>
        /// <param name="grantId">授权人Id</param>
        /// <param name="grantPwd">授权人验证密码</param>
        /// <param name="permissionCode">请求授权URL权限的权限编码</param>
        /// <returns>一次授权码</returns>
        public SDApiResponse<object> CreateLicenseCodeUrl(int grantId, string grantPwd)
        {
            SDApiResponse<object> apiResponse = new SDApiResponse<object>();
            try
            {
                CVerificationPwdInDto cVerificationPwdInDto = new CVerificationPwdInDto()
                {
                    UserId = grantId,
                    Pwd = grantPwd
                };

                HttpResponse httpResponse = SDApiHelper.SDFragmentRequest($"/v1/permissions/verification", HttpMethod.POST, JsonConvert.SerializeObject(cVerificationPwdInDto));

                if (httpResponse != null)
                {
                    apiResponse = httpResponse.AsDeserializeBody<SDApiResponse<object>>();
                }
                else
                {
                    apiResponse = new SDApiResponse<object>();
                    apiResponse.Code = AccountError.Code.ComponentError;
                    apiResponse.Msg = "远程服务器不能访问";
                    apiResponse.Ts = DateTime.Now.Ticks;
                }
            }
            catch (Exception ex)
            {
                apiResponse.Code = AccountError.Code.ComponentError;
                apiResponse.Msg = "系统繁忙，请稍后再试";
            }

            return apiResponse;
        }
    }
}
