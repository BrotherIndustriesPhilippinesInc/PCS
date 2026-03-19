using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("qa_special_evaluation")]
    public class QA_SpecialEvaluation
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

        public string? NeedNoNeed{ get; set; }

        public string? LeadTime { get; set; } //No. of days

        public DateTime? ActualFinishDate { get; set; }

        public string? Result { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string InputBy { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public string? CurrentProcess { get; set; }
    }
}
