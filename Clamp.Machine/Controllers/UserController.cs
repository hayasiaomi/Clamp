using Clamp.AddIns;
using Clamp.Machine.Model;
using Clamp.Machine.Dto.ShanDianView;
using Clamp.Machine.Services;
using Clamp.SDK.Framework;
using Clamp.SDK.Framework.Model;
using Clamp.Webwork;
using Clamp.Webwork.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Clamp.Machine.Factories;

namespace Clamp.Machine.Modules
{
    public class UserController : WebworkController
    {
        private readonly IUserService userServices;

        public UserController() : base("Account")
        {
            userServices = AddInManager.GetEntityService<IUserService>("UserService");
        }

        [Post("users/login")]
        public dynamic Login(string username, string password)
        {
            SDResponse<VMLogin> response = new SDResponse<VMLogin>();

            string test = this.Request.Form.Test;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "账号或密码不能为空";
                return null;
            }

            SDApiResponse<CLoginOutDto> apiResponse = userServices.Login(username, password);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = UserFactory.GetLoginDto(apiResponse.Data);
                    response.Flag = true;
                    response.Code = 200;
                    response.Message = apiResponse.Msg;
                }
                else
                {
                    response.Flag = false;
                    response.Code = AccountError.Code.NotPermissionError;
                    response.Message = "账号或密码不正确";
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


        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="keyword">姓名、账号模糊搜索</param>
        /// <returns></returns>

        [Get("users")]
        public dynamic GetUsers(string keyword)
        {
            SDResponse<List<VMUserSummary>> response = new SDResponse<List<VMUserSummary>>();

            SDApiResponse<List<CUserSummaryDto>> apiResponse = userServices.GetUsers(keyword);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = UserFactory.GetUserSummaryDtoList((apiResponse.Data).OrderByDescending(t => t.CreationTime).ToList());
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

        [Get("users/userId")]
        public dynamic GetUserDetail(int userId)
        {
            SDResponse<VMUserDetail> response = new SDResponse<VMUserDetail>();

            SDApiResponse<CUserDetailDto> apiResponse = userServices.GetUserDetail(userId);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = UserFactory.GetUserDetailDto(apiResponse.Data);
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

        [Post("users")]
        public dynamic AddUser(VMUserIn userInDto)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(userInDto?.UserCode) || string.IsNullOrWhiteSpace(userInDto.Pwd) || string.IsNullOrWhiteSpace(userInDto.OperatorName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "账号或密码不能为空";

                return response.SerializeObject();
            }

            SDApiResponse<object> apiResponse = userServices.AddUser(userInDto);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = apiResponse.Data;
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

        [Put("users/userId")]
        public dynamic AlterUser(VMUserInfoIn userInfoInDto)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(userInfoInDto?.UserCode) || string.IsNullOrWhiteSpace(userInfoInDto.OperatorName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "账号不能为空";
                return response;
            }

            SDApiResponse<object> apiResponse = userServices.AlterUser(userInfoInDto);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = apiResponse.Data;
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

        [Delete("users/userId")]
        public dynamic DeleteUser(int userId, int operatorId, string operatorName)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(operatorName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";

                return response;
            }

            SDApiResponse<object> apiResponse = userServices.DeleteUser(userId, operatorId, operatorName);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = apiResponse.Data;
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

        [Get("permissions/users/code")]
        public dynamic GetUsersByPermissionCode(string code)
        {
            SDResponse<List<VMSimpleUser>> response = new SDResponse<List<VMSimpleUser>>();

            if (string.IsNullOrWhiteSpace(code))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "权限码不能为空";

                return response;
            }

            SDApiResponse<List<CSimpleUserDto>> apiResponse = userServices.GetUsersByPermissionCode(code);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = apiResponse.Data.Where(g => g.IsDefaultAdmin == false).Select(t => (new VMSimpleUser { UserId = t.Id, UserName = t.NickName })).ToList();
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

        [Put("users/userId/role")]
        public dynamic AlterUserRole(VMUserRolePermissionIn userRolePermissionInDto)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(userRolePermissionInDto?.OperatorName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";

                return response;
            }

            SDApiResponse<object> apiResponse = userServices.AlterUserRole(userRolePermissionInDto);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = apiResponse.Data;
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

        [Get("users/verification")]
        public dynamic VerificationUser(VMVerificationUserIn verificationUserInDto)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (verificationUserInDto == null)
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";
                return response;
            }

            SDApiResponse<object> apiResponse = userServices.VerificationUser(verificationUserInDto);

            if (apiResponse.IsSuccess())
            {
                if (apiResponse.Data != null)
                {
                    response.Result = apiResponse.Data;
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

        [Post("LicenseCodePwd/UserId")]
        public dynamic LicenseCodePwdByUserId(int userId, string oldPwd, int operatorId, string operatorName)
        {
            SDResponse<string> response = new SDResponse<string>();

            if (string.IsNullOrWhiteSpace(oldPwd) || string.IsNullOrWhiteSpace(operatorName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";

                return response;
            }

            SDApiResponse<string> apiResponse = userServices.LicenseCodePwdByUserId(userId, oldPwd, operatorId, operatorName);

            if (apiResponse.IsSuccess())
            {
                string key = Guid.NewGuid().ToString("N");

                LicenseCodePwd licenseCode = AccountInfo.LstLicenseCodePwd.FirstOrDefault(t => t.UserId == userId);
                if (licenseCode == null)//不存在创建
                {
                    licenseCode = new LicenseCodePwd()
                    {
                        UserId = userId,
                    };
                    AccountInfo.LstLicenseCodePwd.Add(licenseCode);
                }

                licenseCode.DeadlineTime = DateTime.Now.AddMinutes(30);
                licenseCode.Code = key;
                licenseCode.OperatorId = operatorId;
                licenseCode.OperatorName = operatorName;

                response.Result = key;
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

        [Post("LicenseCodePwd/PermissionCode")]
        public dynamic LicenseCodePwdByPermissionCode(int userId, int operatorId, string operatorName, string permissionCode)
        {
            SDResponse<string> response = new SDResponse<string>();

            if (string.IsNullOrWhiteSpace(permissionCode) || string.IsNullOrWhiteSpace(operatorName))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";

                return response;
            }

            SDApiResponse<string> apiResponse = userServices.LicenseCodePwdByPermissionCode(userId, operatorId, operatorName, permissionCode);

            if (apiResponse.IsSuccess())
            {
                string key = Guid.NewGuid().ToString("N");

                LicenseCodePwd licenseCode = AccountInfo.LstLicenseCodePwd.FirstOrDefault(t => t.UserId == operatorId);

                if (licenseCode == null)//不存在创建
                {
                    licenseCode = new LicenseCodePwd()
                    {
                        UserId = userId
                    };
                    AccountInfo.LstLicenseCodePwd.Add(licenseCode);
                }
                licenseCode.DeadlineTime = DateTime.Now.AddMinutes(30);
                licenseCode.Code = key;
                licenseCode.OperatorId = operatorId;
                licenseCode.OperatorName = operatorName;

                response.Result = key;
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

        [Put("users/Password")]
        public dynamic Password(string messageCode, string password)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(messageCode) || string.IsNullOrWhiteSpace(password))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "授权码或密码不能为空";

                return response;
            }

            LicenseCodePwd licenseCode = AccountInfo.LstLicenseCodePwd.FirstOrDefault(t => t.Code == messageCode);

            if (licenseCode == null)
            {
                response.Flag = false;
                response.Code = AccountError.Code.KeyError;
                response.Message = "授权验证失败";

                return response;
            }

            else if (licenseCode.DeadlineTime < DateTime.Now)//授权码过期
            {
                response.Flag = false;
                response.Code = AccountError.Code.KeyOverTimeError;
                response.Message = "授权验证超时";

                return response;
            }

            SDApiResponse<object> apiResponse = userServices.Password(licenseCode.UserId, licenseCode.OperatorId, licenseCode.OperatorName, password);

            if (apiResponse.IsSuccess())
            {
                AccountInfo.LstLicenseCodePwd.Remove(licenseCode);

                response.Result = apiResponse.Data;
                response.Flag = true;
                response.Message = apiResponse.Msg;
                response.Code = 200;
            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response;
        }

        [Post("LicenseCode/PermissionCode")]
        public dynamic CreateLicenseCodeUrl(int userId, int grantId, string grantPwd, string permissionCode)
        {
            SDResponse<string> response = new SDResponse<string>();

            if (string.IsNullOrWhiteSpace(grantPwd) || string.IsNullOrWhiteSpace(permissionCode))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "参数不能为空";
                return string.Empty;
            }

            SDApiResponse<object> apiResponse = userServices.CreateLicenseCodeUrl(grantId, grantPwd);

            if (apiResponse.IsSuccess())
            {
                LicenseCodeUrl licenseCode = AccountInfo.LstLicenseCodeUrl.FirstOrDefault(t => t.UserId == userId && t.PermissionCode == permissionCode.Trim());
                if (licenseCode == null)
                {
                    licenseCode = new LicenseCodeUrl()
                    {
                        UserId = userId,
                        PermissionCode = permissionCode.Trim()
                    };
                    AccountInfo.LstLicenseCodeUrl.Add(licenseCode);
                }
                string key = Guid.NewGuid().ToString("N");

                licenseCode.DeadlineTime = DateTime.Now.AddMinutes(30);
                licenseCode.Code = key;
                licenseCode.GrantId = grantId;

                response.Result = key;
                response.Flag = true;
                response.Message = apiResponse.Msg;
                response.Code = 200;

            }
            else
            {
                response.Flag = false;
                response.Code = apiResponse.Code;
                response.Message = apiResponse.Msg;
            }

            return response;
        }


        [Put("users/VerifyLicenseCode")]
        public dynamic VerifyLicenseCodeUrl(int userId, string permissionCode, string messageCode)
        {
            SDResponse<object> response = new SDResponse<object>();

            if (string.IsNullOrWhiteSpace(messageCode) || string.IsNullOrWhiteSpace(permissionCode))
            {
                response.Flag = false;
                response.Code = AccountError.Code.ParamsError;
                response.Message = "授权码或权限编码不能为空";
                return response;
            }
            try
            {
                LicenseCodeUrl licenseCode = AccountInfo.LstLicenseCodeUrl.FirstOrDefault(t => t.UserId == userId && t.PermissionCode == permissionCode.Trim() && t.Code == messageCode);
                if (licenseCode == null)
                {
                    response.Flag = false;
                    response.Code = AccountError.Code.KeyError;
                    response.Message = "授权验证失败";
                    return response;
                }

                if (licenseCode.DeadlineTime < DateTime.Now)//授权码过期
                {
                    response.Flag = false;
                    response.Code = AccountError.Code.KeyOverTimeError;
                    response.Message = "授权验证超时";
                    return response;
                }

                AccountInfo.LstLicenseCodeUrl.Remove(licenseCode);
                response.Flag = true;
            }
            catch (Exception ex)
            {
                response.Flag = false;
                response.Code = AccountError.Code.ComponentError;
                response.Message = "系统繁忙，请稍后再试";
            }

            return response;
        }
    }
}
