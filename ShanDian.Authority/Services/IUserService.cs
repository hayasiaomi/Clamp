using System.Collections.Generic;
using ShanDian.Machine.Model;
using ShanDian.Machine.Dto.ShanDianView;
using ShanDian.SDK.Framework.Model;
using ShanDian.SDK.Framework.Services;

namespace ShanDian.Machine.Services
{
    public interface IUserService : IService
    {
        SDApiResponse<CLoginOutDto> Login(string loginName, string pwd);

        SDApiResponse<List<CUserSummaryDto>> GetUsers(string keyword);

        SDApiResponse<CUserDetailDto> GetUserDetail(int userId);

        SDApiResponse<object> AddUser(UserInDto userInDto);

        //void DeleteToken(string token);

        SDApiResponse<object> AlterUser(UserInfoInDto userInfoInDto);

        SDApiResponse<object> AlterUserRole(UserRolePermissionInDto userRolePermissionInDto);

        SDApiResponse<object> DeleteUser(int userId, int operatorId, string operatorName);

        SDApiResponse<List<CSimpleUserDto>> GetUsersByPermissionCode(string code);

        SDApiResponse<object> VerificationUser(VerificationUserInDto verificationUserInDto);

        SDApiResponse<string> LicenseCodePwdByUserId(int userId, string oldPwd, int operatorId, string operatorName);

        SDApiResponse<string> LicenseCodePwdByPermissionCode(int userId, int operatorId, string operatorName, string permissionCode);

        SDApiResponse<object> Password(int userId, int operatorId, string operatorName, string password);

        SDApiResponse<object> CreateLicenseCodeUrl(int grantId, string grantPwd);

        //void VerifyLicenseCodeUrl(int userId, string permissionCode, string messageCode);
    }
}
