using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Permissions
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Permission Permission { get; set; } = new Permission();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Permission = await _context.Permissions.FindAsync(id);
            if (Permission == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"权限 {permission.Name} 已删除";
            return RedirectToPage("./Index");
        }
    }
}
