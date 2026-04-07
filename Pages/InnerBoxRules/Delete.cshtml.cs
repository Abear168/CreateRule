using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.InnerBoxRules
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public InnerBoxRule Rule { get; set; } = new InnerBoxRule();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Rule = await _context.InnerBoxRules.FindAsync(id);
            if (Rule == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var rule = await _context.InnerBoxRules.FindAsync(id);
            if (rule == null)
            {
                return NotFound();
            }

            _context.InnerBoxRules.Remove(rule);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "规则已删除";
            return RedirectToPage("./Index");
        }
    }
}
