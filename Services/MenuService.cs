using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Services
{
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPermissionService _permissionService;

        public MenuService(ApplicationDbContext context, IPermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        public async Task<List<MenuItem>> GetMenuItemsForUserAsync(string userId)
        {
            var allMenuItems = await _context.MenuItems
                .Where(m => m.IsActive)
                .OrderBy(m => m.Order)
                .ToListAsync();

            var userPermissions = await _permissionService.GetUserPermissionsAsync(userId);

            var accessibleMenuItems = allMenuItems
                .Where(m => string.IsNullOrEmpty(m.PermissionCode) || userPermissions.Contains(m.PermissionCode))
                .ToList();

            var rootMenuItems = accessibleMenuItems
                .Where(m => !m.ParentId.HasValue)
                .ToList();

            foreach (var item in rootMenuItems)
            {
                item.Children = accessibleMenuItems
                    .Where(m => m.ParentId == item.Id)
                    .OrderBy(m => m.Order)
                    .ToList();
            }

            return rootMenuItems;
        }
    }
}
