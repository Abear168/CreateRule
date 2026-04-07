using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CreateRule.Models;

namespace CreateRule.Pages.Admin.Users
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public EditModel(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public List<ApplicationRole> AvailableRoles { get; set; } = new List<ApplicationRole>();
        public List<string> UserRoles { get; set; } = new List<string>();

        [BindProperty]
        public string[] SelectedRoles { get; set; } = Array.Empty<string>();

        public class InputModel
        {
            public string Id { get; set; } = string.Empty;

            public string EmployeeId { get; set; } = string.Empty;

            [Required(ErrorMessage = "姓名不能为空")]
            [StringLength(50)]
            public string RealName { get; set; } = string.Empty;

            [Required(ErrorMessage = "邮箱不能为空")]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            public ApprovalStatus ApprovalStatus { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = user.Id,
                EmployeeId = user.EmployeeId,
                Email = user.Email!,
                RealName = user.RealName!,
                ApprovalStatus = user.ApprovalStatus
            };

            AvailableRoles = _roleManager.Roles.ToList();
            UserRoles = (await _userManager.GetRolesAsync(user)).ToList();
            SelectedRoles = UserRoles.ToArray();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AvailableRoles = _roleManager.Roles.ToList();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Email = Input.Email;
            user.RealName = Input.RealName;
            user.ApprovalStatus = Input.ApprovalStatus;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (SelectedRoles.Length > 0)
                {
                    await _userManager.AddToRolesAsync(user, SelectedRoles);
                }

                TempData["SuccessMessage"] = $"用户 {user.EmployeeId} 更新成功";
                return RedirectToPage("./Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"用户 {user.EmployeeId} 密码已重置";
            }
            else
            {
                TempData["ErrorMessage"] = "密码重置失败";
            }

            return RedirectToPage("./Edit", new { id = userId });
        }
    }
}
