using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CreateRule.Data;
using CreateRule.Models;
using CreateRule.Services;

namespace CreateRule.Pages.Admin.Users
{
    [Authorize]
    public class ApproveModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ApproveModel> _logger;

        public ApproveModel(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<ApproveModel> logger)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public ApplicationUser UserProfile { get; set; } = new ApplicationUser();

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            public string UserId { get; set; } = string.Empty;
            public string? Remark { get; set; }
            public bool SendNotification { get; set; } = true;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.ApprovalStatus != ApprovalStatus.Pending)
            {
                return NotFound();
            }

            UserProfile = user;
            Input.UserId = id;
            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null || user.ApprovalStatus != ApprovalStatus.Pending)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            
            user.ApprovalStatus = ApprovalStatus.Approved;
            user.ApprovedAt = DateTime.Now;
            user.ApprovedBy = currentUserId;
            user.ApprovalRemark = Input.Remark;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var auditLog = new AuditLog
                {
                    UserId = user.Id,
                    ActionType = "注册审核",
                    ActionTime = DateTime.Now,
                    OperatorId = currentUserId,
                    Result = "Approved",
                    Remark = Input.Remark
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                if (Input.SendNotification)
                {
                    await _emailService.SendRegistrationApprovedEmailAsync(user.Email!, user.UserName!);
                }

                _logger.LogInformation($"用户 {user.UserName} 审核通过，审核人：{currentUserId}");
                TempData["SuccessMessage"] = $"用户 {user.UserName} 已审核通过";
            }
            else
            {
                TempData["ErrorMessage"] = "审核失败";
            }

            return RedirectToPage("./Pending");
        }

        public async Task<IActionResult> OnPostRejectAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.UserId);
            if (user == null || user.ApprovalStatus != ApprovalStatus.Pending)
            {
                return NotFound();
            }

            var currentUserId = _userManager.GetUserId(User);
            
            user.ApprovalStatus = ApprovalStatus.Rejected;
            user.ApprovedAt = DateTime.Now;
            user.ApprovedBy = currentUserId;
            user.ApprovalRemark = Input.Remark;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var auditLog = new AuditLog
                {
                    UserId = user.Id,
                    ActionType = "注册审核",
                    ActionTime = DateTime.Now,
                    OperatorId = currentUserId,
                    Result = "Rejected",
                    Remark = Input.Remark
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                if (Input.SendNotification && !string.IsNullOrEmpty(Input.Remark))
                {
                    await _emailService.SendRegistrationRejectedEmailAsync(user.Email!, Input.Remark);
                }

                _logger.LogInformation($"用户 {user.UserName} 审核拒绝，审核人：{currentUserId}");
                TempData["SuccessMessage"] = $"用户 {user.UserName} 已拒绝";
            }
            else
            {
                TempData["ErrorMessage"] = "审核失败";
            }

            return RedirectToPage("./Pending");
        }
    }
}
