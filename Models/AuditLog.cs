using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CreateRule.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        
        public string? UserId { get; set; }
        
        [StringLength(50)]
        public string ActionType { get; set; } = string.Empty;
        
        public DateTime ActionTime { get; set; } = DateTime.Now;
        
        public string? OperatorId { get; set; }
        
        [StringLength(20)]
        public string Result { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Remark { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
        
        [ForeignKey("OperatorId")]
        public virtual ApplicationUser? Operator { get; set; }
    }
}
