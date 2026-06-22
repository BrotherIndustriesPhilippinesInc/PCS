using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("new_tooling_category")]
    public class NewToolingCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ControlNumber { get; set; }

        /// <summary>
        /// "Multiple Procurement" | "Supplier Change" | "Localization"
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string AssignedBy { get; set; }
    }
}
