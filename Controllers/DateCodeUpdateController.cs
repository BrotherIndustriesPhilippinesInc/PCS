using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
using PartsControlSystem.ViewModels;
using PartsControlSystem.DTO;

namespace PartsControlSystem.Controllers
{
    public class DateCodeUpdateController : Controller
    {
        private readonly PostgreAppDbContext _context;

        public DateCodeUpdateController(PostgreAppDbContext context)
        {
            _context = context;
        }

        public IActionResult DateCodeUpdate() => View();

        // GET: Load all records — one row per YES activity flag
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var importList = await _context.ImportDatas
                .OrderBy(x => x.ControlNo)
                .ToListAsync();

            var expandedRows = new List<DateCodeRowDto>();

            foreach (var x in importList)
            {
                var activityMap = new Dictionary<string, string>
                {
                    ["Renewal / Additional Mold"] = x.RenewalAdditionalMold,
                    ["New Tooling / Localization"] = x.NewToolingLocalization,
                    ["Transfer Tooling"] = x.TransferTooling,
                    ["Change Material"] = x.ChangeMaterial,
                    ["New Model"] = x.NewModel,
                    ["Non-Concurrent"] = x.NonConcurrent,
                    ["Supplier Change / Localization"] = x.SupplierChangeLocalization,
                    ["Other 4M"] = x.Other4M,
                    ["Multiple Procurement / Localization"] = x.MultipleProcurementLocalization,
                };

                foreach (var (activityName, flag) in activityMap)
                {
                    if (!string.Equals(flag, "YES", StringComparison.OrdinalIgnoreCase))
                        continue;

                    expandedRows.Add(new DateCodeRowDto
                    {
                        Id = x.Id,
                        ControlNo = x.ControlNo,
                        MotherMoldCode = x.MotherMoldCode,
                        ChildPartcode = x.ChildPartcode,
                        PartName = x.PartName,
                        Model = x.Model,
                        Supplier = x.Supplier,
                        MoldMaker = x.MoldMaker,
                        Activity = activityName,
                        ReasonOfChange = x.ReasonOfChange,
                        Section = x.Section,
                        ToolingType = x.ToolingType,
                        ToolingCategory = x.ToolingCategory,
                        DateImported = x.DateImported
                    });
                }
            }

            return Json(new { success = true, data = expandedRows, count = expandedRows.Count });
        }

        // POST: Delete by activity flag — sets the specific flag to null
        // If no flags remain YES after clearing, deletes the entire record
        [HttpPost]
        public async Task<IActionResult> DeleteSelected([FromBody] List<DeleteByActivityDto> items)
        {
            try
            {
                if (items == null || !items.Any())
                    return Json(new { success = false, message = "No items selected." });

                int flagsCleared = 0;
                int recordsDeleted = 0;

                foreach (var item in items)
                {
                    var record = await _context.ImportDatas.FindAsync(item.Id);
                    if (record == null) continue;

                    switch (item.Activity)
                    {
                        case "Renewal / Additional Mold":
                            record.RenewalAdditionalMold = "NO"; break;
                        case "New Tooling / Localization":
                            record.NewToolingLocalization = "NO"; break;
                        case "Transfer Tooling":
                            record.TransferTooling = "NO"; break;
                        case "Change Material":
                            record.ChangeMaterial = "NO"; break;
                        case "New Model":
                            record.NewModel = "NO"; break;
                        case "Non-Concurrent":
                            record.NonConcurrent = "NO"; break;
                        case "Supplier Change / Localization":
                            record.SupplierChangeLocalization = "NO"; break;
                        case "Other 4M":
                            record.Other4M = "NO"; break;
                        case "Multiple Procurement / Localization":
                            record.MultipleProcurementLocalization = "NO"; break;
                        default:
                            continue;
                    }

                    flagsCleared++;

                    var logsToUpdate = await _context.TransactionLogs
                        .Where(t => t.TransactionNumber == record.ControlNo && t.Activity == item.Activity)
                        .ToListAsync();
                        foreach (var log in logsToUpdate)
                        log.Status = "Deleted";


                    bool hasRemainingFlags =
                        record.RenewalAdditionalMold?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true ||
                        record.NewToolingLocalization?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true ||
                        record.TransferTooling?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true ||
                        record.ChangeMaterial?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true ||
                        record.NewModel?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true ||
                        record.NonConcurrent?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true ||
                        record.SupplierChangeLocalization?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true ||
                        record.Other4M?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true ||
                        record.MultipleProcurementLocalization?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true;

                    if (!hasRemainingFlags)
                    {
                        _context.ImportDatas.Remove(record);
                        recordsDeleted++;
                    }
                   
                }

                await _context.SaveChangesAsync();

                string msg = $"{flagsCleared} activity flag(s) cleared.";
                if (recordsDeleted > 0)
                    msg += $" {recordsDeleted} record(s) fully deleted (no remaining activities).";

                return Json(new { success = true, message = msg });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.ToString()
                });
            }
        }

        // POST: Update child part code
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePartCode([FromBody] UpdatePartCodeDto dto)
        {
            if (dto == null || dto.Id <= 0 || string.IsNullOrWhiteSpace(dto.NewPartCode))
                return Json(new { success = false, message = "Invalid data." });

            var record = await _context.ImportDatas.FindAsync(dto.Id);
            if (record == null)
                return Json(new { success = false, message = "Record not found." });

            record.ChildPartcode = dto.NewPartCode.Trim().ToUpper();
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Part code updated.", newPartCode = record.ChildPartcode });
        }

        // POST: Update mother mold code
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMoldCode([FromBody] UpdatePartCodeDto dto)
        {
            if (dto == null || dto.Id <= 0 || string.IsNullOrWhiteSpace(dto.NewPartCode))
                return Json(new { success = false, message = "Invalid data." });

            var record = await _context.ImportDatas.FindAsync(dto.Id);
            if (record == null)
                return Json(new { success = false, message = "Record not found." });

            record.MotherMoldCode = dto.NewPartCode.Trim().ToUpper();
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mold code updated.", newPartCode = record.MotherMoldCode });
        }

        // POST: Update date imported
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDate([FromBody] UpdatePartCodeDto dto)
        {
            if (dto == null || dto.Id <= 0 || string.IsNullOrWhiteSpace(dto.NewPartCode))
                return Json(new { success = false, message = "Invalid data." });

            if (!DateTime.TryParse(dto.NewPartCode, out var newDate))
                return Json(new { success = false, message = "Invalid date format." });

            var record = await _context.ImportDatas.FindAsync(dto.Id);
            if (record == null)
                return Json(new { success = false, message = "Record not found." });

            record.DateImported = DateTime.SpecifyKind(newDate, DateTimeKind.Utc);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Date updated.",
                newPartCode = record.DateImported.ToString("o")
            });
        }
    }
}