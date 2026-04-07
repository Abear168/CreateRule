using CreateRule.Models;

namespace CreateRule.Services
{
    public interface IMenuService
    {
        Task<List<MenuItem>> GetMenuItemsForUserAsync(string userId);
    }
}
