using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Permissions
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "权限代码不能为空")]
            [StringLength(100)]
            public string Code { get; set; } = string.Empty;

            [Required(ErrorMessage = "权限名称不能为空")]
            [StringLength(100)]
            public string Name { get; set; } = string.Empty;

            [StringLength(50)]
            public string? Module { get; set; }

            [StringLength(500)]
            public string? Description { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingPermission = _context.Permissions.Any(p => p.Code == Input.Code);
            if (existingPermission)
            {
                ModelState.AddModelError(string.Empty, "权限代码已存在");
                return Page();
            }

            var permission = new Permission
            {
                Code = Input.Code,
                Name = Input.Name,
                Module = Input.Module,
                Description = Input.Description,
                CreatedAt = DateTime.Now
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"权限 {Input.Name} 创建成功";
            return RedirectToPage("./Index");
        }
    }
}
