using PartsControlSystem.Models;

namespace PartsControlSystem.ViewModels
{
    public class LeadTimeSettingViewModel
    {
        // From lead_times table (Renewal)
        public List<LeadTime> RenewalLeadTimes { get; set; } = new();

        // From new_tooling_process_mapping table
        public List<NewToolingProcessMapping> MultipleProcurementSteps { get; set; } = new();
        public List<NewToolingProcessMapping> SupplierChangeSteps { get; set; } = new();
        public List<NewToolingProcessMapping> LocalizationSteps { get; set; } = new();

        // From change_material_process_mapping
        public List<ChangeMaterialProcessMapping> ChangeMaterialSteps { get; set; } = new();

        // From Other4MProcessMappings table
        public List<Other4MProcessMapping> Other4MSteps { get; set; } = new();
    }

    // Used for bulk-save JSON payloads
    public class LeadTimeUpdateDto
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
    }
}