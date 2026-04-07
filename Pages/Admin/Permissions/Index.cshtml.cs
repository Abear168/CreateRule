using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Permissions
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Dictionary<string, List<Permission>> PermissionsByModule { get; set; } = new Dictionary<string, List<Permission>>();

        public async Task OnGetAsync()
        {
            var permissions = await _context.Permissions
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();

            PermissionsByModule = permissions
                .GroupBy(p => p.Module ?? "其他")
                .ToDictionary(g => g.Key, g => g.ToList());
        }
    }
}
