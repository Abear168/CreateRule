using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Roles
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public List<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();

        public async Task OnGetAsync()
        {
            Roles = await _roleManager.Roles
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

        public int GetUserCount(string roleId)
        {
            return _context.UserRoles.Count(ur => ur.RoleId == roleId);
        }
    }
}
