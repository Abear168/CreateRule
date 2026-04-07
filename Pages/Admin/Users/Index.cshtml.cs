using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Users
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public int PageIndex { get; set; } = 1;
        public int TotalPages { get; set; }
        public string? SearchString { get; set; }
        public string? StatusFilter { get; set; }

        private const int PageSize = 10;

        public async Task OnGetAsync(string? searchString, string? statusFilter, int pageIndex = 1)
        {
            SearchString = searchString;
            StatusFilter = statusFilter;
            PageIndex = pageIndex;

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u => u.EmployeeId!.Contains(searchString) ||
                                         u.RealName!.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<ApprovalStatus>(statusFilter, out var status))
            {
                query = query.Where(u => u.ApprovalStatus == status);
            }

            var totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            Users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}
