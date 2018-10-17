using System.Collections.Generic;
using Clamp.Machine.Model;
using Clamp.Machine.Dto.ShanDianView;
using Clamp.SDK.Framework.Model;
using Clamp.SDK.Framework.Services;

namespace Clamp.Machine.Services
{
    public interface IUserService : IService
    {
        SDApiResponse<CLoginOutDto> Login(string loginName, string pwd);

        SDApiResponse<List<CUserSummaryDto>> GetUsers(string keyword);

        SDApiResponse<CUserDetailDto> GetUserDetail(int userId);

        SDApiResponse<object> AddUser(VMUserIn userInDto);

        //void DeleteToken(string token);

        SDApiResponse<object> AlterUser(VMUserInfoIn userInfoInDto);

        SDApiResponse<object> AlterUserRole(VMUserRolePermissionIn userRolePermissionInDto);

        SDApiResponse<object> DeleteUser(int userId, int operatorId, string operatorName);

        SDApiResponse<List<CSimpleUserDto>> GetUsersByPermissionCode(string code);

        SDApiResponse<object> VerificationUser(VMVerificationUserIn verificationUserInDto);

        SDApiResponse<string> LicenseCodePwdByUserId(int userId, string oldPwd, int operatorId, string operatorName);

        SDApiResponse<string> LicenseCodePwdByPermissionCode(int userId, int operatorId, string operatorName, string permissionCode);

        SDApiResponse<object> Password(int userId, int operatorId, string operatorName, string password);

        SDApiResponse<object> CreateLicenseCodeUrl(int grantId, string grantPwd);

        //void VerifyLicenseCodeUrl(int userId, string permissionCode, string messageCode);
    }
}
