using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using CreateRule.Data;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Roles
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public CreateModel(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public Dictionary<string, List<Permission>> PermissionsByModule { get; set; } = new Dictionary<string, List<Permission>>();

        [BindProperty]
        public int[] SelectedPermissions { get; set; } = Array.Empty<int>();

        public class InputModel
        {
            [Required(ErrorMessage = "角色名称不能为空")]
            [StringLength(50)]
            public string Name { get; set; } = string.Empty;

            [StringLength(200)]
            public string? Description { get; set; }
        }

        public async Task OnGetAsync()
        {
            var permissions = await _context.Permissions.OrderBy(p => p.Module).ThenBy(p => p.Name).ToListAsync();
            PermissionsByModule = permissions
                .GroupBy(p => p.Module ?? "其他")
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await OnGetAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var role = new ApplicationRole
            {
                Name = Input.Name,
                Description = Input.Description,
                CreatedAt = DateTime.Now
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                if (SelectedPermissions.Length > 0)
                {
                    foreach (var permissionId in SelectedPermissions)
                    {
                        _context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permissionId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"角色 {Input.Name} 创建成功";
                return RedirectToPage("./Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
