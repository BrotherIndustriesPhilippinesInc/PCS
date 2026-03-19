using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PartsControlSystem.Models
{
    [Table("mp2_tooling_po_issuance")]
    public class MP2_ToolingPoIssuance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ControlNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Section { get; set; }

        [Required]
        [StringLength(200)]
        public string Activity { get; set; }

        [NotMapped]
        public DateTime? LimitDate { get; set; }

        [NotMapped]
        public int? RemainingDaysUntilLimit { get; set; }

        public DateTime? TargetIssueDate { get; set; }
        public DateTime? ActualIssueDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string InputBy { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public string? CurrentProcess { get; set; }
    }
}
