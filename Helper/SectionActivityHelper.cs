using PartsControlSystem.Data;

namespace PartsControlSystem.Helpers
{
    public static class SectionActivityHelper
    {
        public static List<string> GetAllowedActivitiesForSection(PostgreAppDbContext dbContext, string section)
        {
            if (string.IsNullOrWhiteSpace(section))
                return new List<string>();

            var allowed = new List<string>();

            if (dbContext.LeadTimes.Any(x => x.Section == section))
                allowed.Add("Renewal / Additional Mold");

            if (dbContext.NewToolingProcessMappings.Any(x => x.Category == "Localization" && x.Section == section))
                allowed.Add("New Tooling / Localization");

            if (dbContext.NewToolingProcessMappings.Any(x => x.Category == "Supplier Change" && x.Section == section))
                allowed.Add("Supplier Change / Localization");

            if (dbContext.NewToolingProcessMappings.Any(x => x.Category == "Multiple Procurement" && x.Section == section))
                allowed.Add("Multiple Procurement / Localization");

            if (dbContext.ChangeMaterialProcessMappings.Any(x => x.Section == section))
                allowed.Add("Change Material");

            // Other4MProcessMappings has no Section column — hardcoded to IQC in the view
            if (string.Equals(section, "IQC", StringComparison.OrdinalIgnoreCase))
                allowed.Add("Other 4M");

            return allowed;
        }
    }
}