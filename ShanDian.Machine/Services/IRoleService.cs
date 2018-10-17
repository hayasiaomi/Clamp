using System.Collections.Generic;
using ShanDian.Machine.Model;
using ShanDian.Machine.Dto.ShanDianView;
using ShanDian.SDK.Framework.Model;
using ShanDian.SDK.Framework.Services;

namespace ShanDian.Machine.Services
{
    public interface IRoleService : IService
    {
        SDApiResponse<List<CPermissionCategoryDto>> GetPermissionsConfigure();

        SDApiResponse<List<CRoleSummaryDto>> GetRoleSummary();

        SDApiResponse<List<CPermissionCategoryDto>> GetRolePermissionsCategory(int roleId);

        SDApiResponse<CRoleSummaryDto> AddRolePermission(VMRolePermissionIn rolePermissionInDto);

        SDApiResponse<object> AlterRolePermission(VMRolePermissionIn rolePermissionInDto);

        SDApiResponse<object> AlterRoleUsers(VMUseArray useArray);

        SDApiResponse<object> DeleteRole(int roleId, int operatorId, string operatorName);
    }
}
