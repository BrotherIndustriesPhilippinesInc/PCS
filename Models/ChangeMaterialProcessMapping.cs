using PartsControlSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("change_material_process_mappings")]
    public class ChangeMaterialProcessMapping
    {
        [Key]
        public int Id { get; set; }
        public string ProcessStep { get; set; }
        public int StepOrder { get; set; }
        public string Section { get; set; }
        public int LeadTime { get; set; }
    }
}



namespace PartsControlSystem.Data
{
    public static class ChangeMaterialProcessMappingSeed
    {
        public static List<ChangeMaterialProcessMapping> GetSeedData() => new()
        {
            new ChangeMaterialProcessMapping { Id = 1, ProcessStep = "Material LOA",                 StepOrder = 1, Section = "MP1", LeadTime = 21  }, // 3-4 weeks
            new ChangeMaterialProcessMapping { Id = 2, ProcessStep = "Kataken PH Sample Submission", StepOrder = 2, Section = "IQC", LeadTime = 0   }, // Manual Input
            new ChangeMaterialProcessMapping { Id = 3, ProcessStep = "Kataken Evaluation Approval",  StepOrder = 3, Section = "IQC", LeadTime = 7   }, // 7 days
            new ChangeMaterialProcessMapping { Id = 4, ProcessStep = "QA Evaluation",                StepOrder = 4, Section = "IQC", LeadTime = 10  }, // 10 days
            new ChangeMaterialProcessMapping { Id = 5, ProcessStep = "DE Evaluation",                StepOrder = 5, Section = "IQC", LeadTime = 10  }, // 10 days
            new ChangeMaterialProcessMapping { Id = 6, ProcessStep = "Test Run",                     StepOrder = 6, Section = "IQC", LeadTime = 2   }, // 2 days
            new ChangeMaterialProcessMapping { Id = 7, ProcessStep = "Implementation Date",          StepOrder = 7, Section = "MP1", LeadTime = 0   }, // Depends on DCI
            new ChangeMaterialProcessMapping { Id = 8, ProcessStep = "First Delivery Date",          StepOrder = 8, Section = "MP1", LeadTime = 0   }, // Depends on DCI
        };
    }
}
