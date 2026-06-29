// Services/StatusSyncBackgroundService.cs
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Helpers;

namespace PartsControlSystem.Services
{
    public class StatusSyncBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StatusSyncBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1); // adjust as needed

        public StatusSyncBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<StatusSyncBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<PostgreAppDbContext>();

                    await SyncStatusesAsync(dbContext);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during scheduled status sync.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task SyncStatusesAsync(PostgreAppDbContext dbContext)
        {
            var allLogs = await dbContext.TransactionLogs.ToListAsync();

            var latestProcesses = await dbContext.ActivityCurrentProcesses
                .GroupBy(x => x.ControlNumber)
                .Select(g => g.OrderByDescending(x => x.UpdateAt).First())
                .ToListAsync();

            var leadTimes = await dbContext.LeadTimes.ToListAsync();
            var newToolingMappings = await dbContext.NewToolingProcessMappings.ToListAsync();
            var changeMaterialMappings = await dbContext.ChangeMaterialProcessMappings.ToListAsync();
            var other4MMappings = await dbContext.Other4MProcessMappings.ToListAsync();
            var today = DateTime.UtcNow;

            var latestPerGroup = allLogs
                .GroupBy(x => new { x.TransactionNumber, x.Activity })
                .Select(g => g.OrderByDescending(x => x.InputDate).First())
                .ToList();

            int updatedCount = 0;

            foreach (var log in latestPerGroup)
            {
                if (log.Status == "Deleted")
                    continue;

                var latestProcess = latestProcesses
                    .FirstOrDefault(p => p.ControlNumber == log.TransactionNumber);

                string actualCurrentProcess = latestProcess?.CurrentProcess ?? log.CurrentProcess;
                bool isCompleted = IsCompletedStatic(log.Activity, actualCurrentProcess);

                string resolvedStatus = ActivityComputationHelper.ResolveTransactionLogStatus(
                    isCompleted, actualCurrentProcess, log.InputDate, log.Activity,
                    leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today);

                if (log.Status != resolvedStatus)
                {
                    log.Status = resolvedStatus;
                    updatedCount++;
                }
            }

            if (updatedCount > 0)
            {
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Status sync completed. {Count} row(s) updated.", updatedCount);
            }
        }

        private static bool IsCompletedStatic(string activity, string currentProcess)
        {
            if (string.IsNullOrWhiteSpace(currentProcess)) return false;

            if (activity == "Renewal / Additional Mold")
                return currentProcess == "MP2-PDC";
            if (activity == "Change Material")
                return currentProcess == "First Delivery Date";
            if (activity == "Other 4M")
                return currentProcess == "FIRST DELIVERY DATE";
            if (activity == "New Tooling / Localization"
                || activity == "Multiple Procurement / Localization"
                || activity == "Supplier Change / Localization")
                return currentProcess == "Completed";

            return currentProcess == "Completed";
        }
    }
}