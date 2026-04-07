using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using CreateRule.Models;

namespace CreateRule.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "工号不能为空")]
            public string EmployeeId { get; set; } = string.Empty;

            [Required(ErrorMessage = "密码不能为空")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(Input.EmployeeId);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "工号或密码错误");
                    return Page();
                }

                if (user.ApprovalStatus != ApprovalStatus.Approved)
                {
                    if (user.ApprovalStatus == ApprovalStatus.Pending)
                    {
                        ModelState.AddModelError(string.Empty, "您的账号正在审核中，请等待管理员审核");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "您的账号审核未通过，请联系管理员");
                    }
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.EmployeeId, Input.Password, false, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    user.LastLoginAt = DateTime.Now;
                    await _signInManager.UserManager.UpdateAsync(user);

                    _logger.LogInformation($"用户 {Input.EmployeeId} 登录成功");
                    return LocalRedirect(returnUrl);
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning($"用户 {Input.EmployeeId} 账号被锁定");
                    ModelState.AddModelError(string.Empty, "账号已被锁定，请稍后再试");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "工号或密码错误");
                    return Page();
                }
            }

            return Page();
        }
    }
}
