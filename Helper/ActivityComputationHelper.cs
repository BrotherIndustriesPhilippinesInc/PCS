using PartsControlSystem.Models;

namespace PartsControlSystem.Helpers
{
    public static class ActivityComputationHelper
    {
        // Maps the 3 shared-table activities to their NewToolingProcessMapping.Category
        private static readonly Dictionary<string, string> NewToolingCategoryMap = new()
        {
            ["New Tooling / Localization"] = "Localization",
            ["Supplier Change / Localization"] = "Supplier Change",
            ["Multiple Procurement / Localization"] = "Multiple Procurement",
        };

        /// <summary>
        /// Resolves the lead-time-days for a given activity + process step,
        /// pulling from the correct mapping table per activity.
        /// </summary>
        public static double? ResolveLeadTimeDays(
            string activityName,
            string? processStep,
            List<LeadTime> leadTimes,
            List<NewToolingProcessMapping> newToolingMappings,
            List<ChangeMaterialProcessMapping> changeMaterialMappings,
            List<Other4MProcessMapping> other4MMappings)
        {
            if (string.IsNullOrWhiteSpace(processStep)) return null;

            if (NewToolingCategoryMap.TryGetValue(activityName, out var category))
            {
                var match = newToolingMappings
                    .FirstOrDefault(m => m.Category == category && m.ProcessStep == processStep);
                return match == null ? null : (double)match.LeadTimeDays;
            }

            if (activityName == "Change Material")
                return (double?)changeMaterialMappings
                    .FirstOrDefault(c => c.ProcessStep == processStep)?.LeadTime;

            if (activityName == "Other 4M")
                return other4MMappings
                    .FirstOrDefault(o => o.ProcessStep == processStep)?.LeadTimeDays;

            // Renewal / Additional Mold, Transfer Tooling, New Model, Non-Concurrent
            return (double?)leadTimes
                .FirstOrDefault(lt => lt.Activity == processStep)?.LeadTimeValue;
        }

        /// <summary>
        /// Resolves the latest TransactionLogs entry strictly scoped to (controlNo, activityName) —
        /// never mixes process steps across activities for the same ControlNo.
        /// </summary>
        public static TransactionLogs? GetLatestLogForActivity(
            string controlNo,
            string activityName,
            List<TransactionLogs> latestLogsPerActivity)
            => latestLogsPerActivity.FirstOrDefault(
                l => l.TransactionNumber == controlNo && l.Activity == activityName);

        /// <summary>
        /// Single source of truth for status: "Finished" / "Ongoing" / "Delay".
        /// Rule: 0 or null lead time => never Delay, always Ongoing (not yet configured).
        /// </summary>
        public static string ResolveStatus(
            bool isCompleted,
            TransactionLogs? latestLog,
            string activityName,
            List<LeadTime> leadTimes,
            List<NewToolingProcessMapping> newToolingMappings,
            List<ChangeMaterialProcessMapping> changeMaterialMappings,
            List<Other4MProcessMapping> other4MMappings,
            DateTime today)
        {
            if (isCompleted) return "Finished";
            if (latestLog == null) return "Ongoing";

            var leadTimeDays = ResolveLeadTimeDays(
                activityName, latestLog.CurrentProcess,
                leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings);

            if (leadTimeDays == null || leadTimeDays <= 0)
                return "Ongoing"; // not yet set / zero => can't be judged late

            if (latestLog.InputDate == null)
                return "Ongoing"; // no date logged yet => can't be judged late

            var deadline = latestLog.InputDate.Value.AddDays(leadTimeDays.Value);
            return deadline < today ? "Delay" : "Ongoing";
        }
        /// <summary>
        /// Status resolver for TransactionLogs page — completion comes from a
        /// process-step string match (IsCompleted) rather than the dedicated
        /// completion tables used by Dashboard/AllPartsData, but Delay/In Progress
        /// uses the SAME lead-time logic so "late" means the same thing everywhere.
        /// </summary>
        public static string ResolveTransactionLogStatus(
            bool isCompleted,
            string? currentProcess,
            DateTime? inputDate,
            string activityName,
            List<LeadTime> leadTimes,
            List<NewToolingProcessMapping> newToolingMappings,
            List<ChangeMaterialProcessMapping> changeMaterialMappings,
            List<Other4MProcessMapping> other4MMappings,
            DateTime today)
        {
            if (isCompleted) return "Completed";

            var leadTimeDays = ResolveLeadTimeDays(
                activityName, currentProcess,
                leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings);

            if (leadTimeDays == null || leadTimeDays <= 0) return "In Progress";
            if (inputDate == null) return "In Progress";

            var deadline = inputDate.Value.AddDays(leadTimeDays.Value);
            return deadline < today ? "Delay" : "In Progress";
        }
    }
}