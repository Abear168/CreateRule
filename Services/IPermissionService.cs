using CreateRule.Models;

namespace CreateRule.Services
{
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(string userId, string permissionCode);
        Task<List<string>> GetUserPermissionsAsync(string userId);
    }
}
