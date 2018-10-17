using System.Collections.Generic;
using Clamp.Machine.Model;
using Clamp.Machine.Dto.ShanDianView;
using Clamp.SDK.Framework.Model;
using Clamp.SDK.Framework.Services;

namespace Clamp.Machine.Services
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
