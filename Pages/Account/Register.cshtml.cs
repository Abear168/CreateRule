using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CreateRule.Models;

namespace CreateRule.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(UserManager<ApplicationUser> userManager, ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "工号不能为空")]
            [StringLength(20, MinimumLength = 3, ErrorMessage = "工号长度必须在3-20个字符之间")]
            public string EmployeeId { get; set; } = string.Empty;

            [Required(ErrorMessage = "姓名不能为空")]
            [StringLength(50, ErrorMessage = "姓名长度不能超过50个字符")]
            public string RealName { get; set; } = string.Empty;

            [Required(ErrorMessage = "邮箱不能为空")]
            [EmailAddress(ErrorMessage = "邮箱格式不正确")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "密码不能为空")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度至少6个字符")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var existingEmployeeId = await _userManager.FindByNameAsync(Input.EmployeeId);
                if (existingEmployeeId != null)
                {
                    ModelState.AddModelError(string.Empty, "工号已被注册");
                    return Page();
                }

                var existingEmail = await _userManager.FindByEmailAsync(Input.Email);
                if (existingEmail != null)
                {
                    ModelState.AddModelError(string.Empty, "邮箱已被注册");
                    return Page();
                }

                var user = new ApplicationUser
                {
                    UserName = Input.EmployeeId,
                    EmployeeId = Input.EmployeeId,
                    Email = Input.Email,
                    RealName = Input.RealName,
                    CreatedAt = DateTime.Now,
                    ApprovalStatus = ApprovalStatus.Pending
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"用户 {Input.EmployeeId} 注册成功，等待审核");

                    return RedirectToPage("./RegisterPending");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}
