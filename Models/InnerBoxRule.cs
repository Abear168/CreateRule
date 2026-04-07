using System.ComponentModel.DataAnnotations;

namespace CreateRule.Models
{
    public class InnerBoxRule
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string WorkOrder { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Template { get; set; } = string.Empty;

        [StringLength(100)]
        public string Prefix { get; set; } = string.Empty;

        public int PrefixLength { get; set; } = 4;

        [StringLength(100)]
        public string Constant { get; set; } = string.Empty;

        public int SequenceLength { get; set; } = 4;

        public int SequenceStart { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? CreatedById { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedById { get; set; }
    }
}
