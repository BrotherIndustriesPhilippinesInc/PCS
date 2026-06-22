using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("change_material_processes")]
    public class ChangeMaterialProcess
    {
        [Key]
        public int Id { get; set; }
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public string ProcessStep { get; set; }
        public int StepOrder { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public string CurrentProcess { get; set; }
        public string InputBy { get; set; }
        public DateTime CreateDate { get; set; }
    }
}