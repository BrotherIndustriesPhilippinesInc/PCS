using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
using PartsControlSystem.ViewModels;

namespace PartsControlSystem.Controllers
{
    public class LeadTimeController : Controller
    {
        private readonly PostgreAppDbContext _context;

        public LeadTimeController(PostgreAppDbContext context)
        {
            _context = context;
        }

        // GET: /LeadTime/LeadTimeSetting
        public async Task<IActionResult> LeadTimeSetting()
        {
            var viewModel = new LeadTimeSettingViewModel
            {
                // Renewal lead times (from lead_times table)
                RenewalLeadTimes = await _context.LeadTimes
                    .OrderBy(x => x.Section)
                    .ThenBy(x => x.Activity)
                    .ToListAsync(),

                // New Tooling lead times grouped by category (from new_tooling_process_mapping)
                MultipleProcurementSteps = await _context.NewToolingProcessMappings
                    .Where(x => x.Category == "Multiple Procurement")
                    .OrderBy(x => x.StepOrder)
                    .ToListAsync(),

                SupplierChangeSteps = await _context.NewToolingProcessMappings
                    .Where(x => x.Category == "Supplier Change")
                    .OrderBy(x => x.StepOrder)
                    .ToListAsync(),

                LocalizationSteps = await _context.NewToolingProcessMappings
                    .Where(x => x.Category == "Localization")
                    .OrderBy(x => x.StepOrder)
                    .ToListAsync(),

                ChangeMaterialSteps = await _context.ChangeMaterialProcessMappings
                    .OrderBy(x => x.StepOrder)
                    .ToListAsync(),
            };

            return View(viewModel);
        }

        // POST: Update a single Renewal lead time
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRenewalLeadTime(int id, decimal leadTimeValue)
        {
            var record = await _context.LeadTimes.FindAsync(id);
            if (record == null)
                return Json(new { success = false, message = "Record not found." });

            record.LeadTimeValue = leadTimeValue;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Lead time updated successfully." });
        }

        // POST: Update a single NewToolingProcessMapping lead time
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateToolingLeadTime(int id, decimal leadTimeDays)
        {
            var record = await _context.NewToolingProcessMappings.FindAsync(id);
            if (record == null)
                return Json(new { success = false, message = "Record not found." });

            record.LeadTimeDays = leadTimeDays;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Lead time updated successfully." });
        }

        // POST: Bulk save all Renewal lead times
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAllRenewal([FromBody] List<LeadTimeUpdateDto> updates)
        {
            if (updates == null || !updates.Any())
                return Json(new { success = false, message = "No data received." });

            foreach (var update in updates)
            {
                var record = await _context.LeadTimes.FindAsync(update.Id);
                if (record != null)
                    record.LeadTimeValue = update.Value;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"{updates.Count} renewal lead time(s) saved." });
        }

        // POST: Bulk save all Tooling lead times
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAllTooling([FromBody] List<LeadTimeUpdateDto> updates)
        {
            if (updates == null || !updates.Any())
                return Json(new { success = false, message = "No data received." });

            foreach (var update in updates)
            {
                var record = await _context.NewToolingProcessMappings.FindAsync(update.Id);
                if (record != null)
                    record.LeadTimeDays = update.Value;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"{updates.Count} tooling lead time(s) saved." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAllChangeMaterial([FromBody] List<LeadTimeUpdateDto> updates)
        {
            if (updates == null || !updates.Any())
                return Json(new { success = false, message = "No data received." });

            foreach (var update in updates)
            {
                var record = await _context.ChangeMaterialProcessMappings.FindAsync(update.Id);
                if (record != null)
                    record.LeadTime = update.Value;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"{updates.Count} Change Material lead time(s) saved." });
        }
    }
}