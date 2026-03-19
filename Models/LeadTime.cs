using System.ComponentModel.DataAnnotations.Schema;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PartsControlSystem.Models
{
    public class LeadTime
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("activity")]
        public string Activity { get; set; } = string.Empty;

        // Section name
        [Column("section")]
        public string Section { get; set; } = string.Empty;

        // Lead Time value
        [Column("lead_time")]
        public decimal LeadTimeValue { get; set; } = 0;
    }
}
