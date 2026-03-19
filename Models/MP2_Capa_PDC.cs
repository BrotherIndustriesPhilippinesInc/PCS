using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PartsControlSystem.Models
{
    [Table("mp2_capa_pdc")]
    public class MP2_Capa_PDC
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ControlNumber { get; set; }

        public DateTime? PartsShortageDate { get; set; }
        public DateTime? TargetMoldUsageDate { get; set; }

        [StringLength(100)]
        public string InputBy { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    }
}
