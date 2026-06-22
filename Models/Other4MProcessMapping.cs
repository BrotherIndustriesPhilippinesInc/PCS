using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    // Fixed 13-step sequence for the "Other 4M" activity.
    // Unlike Change Material / New Tooling, this flow does NOT vary per Section —
    // it is always handled by IQC end-to-end, so no Section column here.
    [Table("Other4MProcessMappings")]
    public class Other4MProcessMapping
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProcessStep { get; set; } = string.Empty;

        public int StepOrder { get; set; }

        // Numeric lead time in days, when applicable (null when manual/dependent)
        public int? LeadTimeDays { get; set; }

        // Free-text description of the lead time rule, e.g.
        // "5 days after 4M form received", "Manual Input/0",
        // "Depends on DCI implementation / current material stocks depletion"
        public string? LeadTimeNote { get; set; }
    }
}