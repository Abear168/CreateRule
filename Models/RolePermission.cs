using System.ComponentModel.DataAnnotations.Schema;

namespace CreateRule.Models
{
    public class RolePermission
    {
        public string RoleId { get; set; } = string.Empty;
        public int PermissionId { get; set; }
        
        [ForeignKey("RoleId")]
        public virtual ApplicationRole? Role { get; set; }
        
        [ForeignKey("PermissionId")]
        public virtual Permission? Permission { get; set; }
    }
}
