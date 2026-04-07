using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Users
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CreateModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public List<ApplicationRole> AvailableRoles { get; set; } = new List<ApplicationRole>();

        [BindProperty]
        public string[] SelectedRoles { get; set; } = Array.Empty<string>();

        public class InputModel
        {
            [Required(ErrorMessage = "工号不能为空")]
            [StringLength(20, MinimumLength = 3)]
            public string EmployeeId { get; set; } = string.Empty;

            [Required(ErrorMessage = "姓名不能为空")]
            [StringLength(50)]
            public string RealName { get; set; } = string.Empty;

            [Required(ErrorMessage = "邮箱不能为空")]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "密码不能为空")]
            [StringLength(100, MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "两次密码不一致")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public async Task OnGetAsync()
        {
            AvailableRoles = _roleManager.Roles.ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AvailableRoles = _roleManager.Roles.ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new ApplicationUser
            {
                UserName = Input.EmployeeId,
                EmployeeId = Input.EmployeeId,
                Email = Input.Email,
                RealName = Input.RealName,
                CreatedAt = DateTime.Now,
                ApprovalStatus = ApprovalStatus.Approved,
                ApprovedAt = DateTime.Now,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                if (SelectedRoles.Length > 0)
                {
                    await _userManager.AddToRolesAsync(user, SelectedRoles);
                }

                TempData["SuccessMessage"] = $"用户 {Input.EmployeeId} 创建成功";
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
