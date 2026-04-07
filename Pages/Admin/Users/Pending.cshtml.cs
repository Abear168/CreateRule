using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Users
{
    [Authorize]
    public class PendingModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PendingModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ApplicationUser> PendingUsers { get; set; } = new List<ApplicationUser>();

        public async Task OnGetAsync()
        {
            PendingUsers = await _context.Users
                .Where(u => u.ApprovalStatus == ApprovalStatus.Pending)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }
    }
}
