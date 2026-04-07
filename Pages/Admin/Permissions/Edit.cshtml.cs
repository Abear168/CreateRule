using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Permissions
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            public int Id { get; set; }

            public string Code { get; set; } = string.Empty;

            [Required(ErrorMessage = "权限名称不能为空")]
            [StringLength(100)]
            public string Name { get; set; } = string.Empty;

            [StringLength(50)]
            public string? Module { get; set; }

            [StringLength(500)]
            public string? Description { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = permission.Id,
                Code = permission.Code,
                Name = permission.Name,
                Module = permission.Module,
                Description = permission.Description
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var permission = await _context.Permissions.FindAsync(Input.Id);
            if (permission == null)
            {
                return NotFound();
            }

            permission.Name = Input.Name;
            permission.Module = Input.Module;
            permission.Description = Input.Description;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"权限 {Input.Name} 更新成功";
            return RedirectToPage("./Index");
        }
    }
}
