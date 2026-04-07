using Microsoft.AspNetCore.Identity;

namespace CreateRule.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string? RealName { get; set; }
        public string? Department { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }

        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ApprovalRemark { get; set; }
    }

    public enum ApprovalStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
