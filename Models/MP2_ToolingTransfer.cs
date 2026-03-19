using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("mp2_tooling_transfer")]
    public class MP2_ToolingTransfer
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

        public string? TransferLeadTime { get; set; }

        public DateTime? TargetArrivalDate { get; set; }

        public DateTime? ActualArrivalDate { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        [StringLength(100)]
        public string InputBy { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        public string? CurrentProcess { get; set; }
    }
}
