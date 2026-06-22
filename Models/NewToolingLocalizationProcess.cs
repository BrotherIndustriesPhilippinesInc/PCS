using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PartsControlSystem.Models
{
    [Table("new_tooling_localization_process")]
    public class NewToolingLocalizationProcess
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
        [Required]
        [StringLength(100)]
        public string Category { get; set; }
        [Required]
        [StringLength(200)]
        public string ProcessStep { get; set; }
        public int StepOrder { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        [StringLength(100)]
        public string? Result { get; set; }
        [StringLength(200)]
        public string? ReferenceNo { get; set; }
        [StringLength(500)]
        public string? Remarks { get; set; }

        // ← NEW: dedicated column for Step 16 Final PO Issued Date
        public DateTime? FinalPOIssuedDate { get; set; }

        [StringLength(100)]
        public string InputBy { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        [StringLength(200)]
        public string? CurrentProcess { get; set; }
    }
}