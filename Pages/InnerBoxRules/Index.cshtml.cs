using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;
using CreateRule.Services;

namespace CreateRule.Pages.InnerBoxRules
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<InnerBoxRule> Rules { get; set; } = new List<InnerBoxRule>();
        public List<string> Previews { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            Rules = await _context.InnerBoxRules
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            Previews = Rules.Select(r => RuleGenerationService.GeneratePreview(r, DateTime.Now)).ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var rule = await _context.InnerBoxRules.FindAsync(id);
            if (rule != null)
            {
                _context.InnerBoxRules.Remove(rule);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "规则删除成功";
            }
            return RedirectToPage();
        }
    }
}