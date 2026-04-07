using System.ComponentModel.DataAnnotations;

namespace CreateRule.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Icon { get; set; }
        
        [StringLength(200)]
        public string? Url { get; set; }
        
        public int? ParentId { get; set; }
        
        public int Order { get; set; } = 0;
        
        [StringLength(100)]
        public string? PermissionCode { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public virtual MenuItem? Parent { get; set; }
        public virtual ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
    }
}
