using System.Diagnostics;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PartsControlSystem.Data;
using PartsControlSystem.Models;

namespace PartsControlSystem.Controllers
{
    public class ImportDataController : Controller
    {
        private readonly PostgreAppDbContext _dbContext;

        public ImportDataController(PostgreAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult DataEntry()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveImportData([FromBody] ImportData importData)
        {
            if (importData == null)
                return Json(new { success = false, message = "Invalid data." });

            try
            {
                var section = User.FindFirst("Section")?.Value;

                if (string.IsNullOrEmpty(section))
                    return Json(new { success = false, message = "Section not found. Please re-login." });

                importData.Section = section;

                // ── Generate Control No ──────────────────────────────────────
                var today = DateTime.Now.ToString("yyyyMMdd");
                var prefix = $"IMP-DATA-{today}-";

                var lastControlNo = await _dbContext.ImportDatas
                    .Where(x => x.ControlNo.StartsWith(prefix))
                    .OrderByDescending(x => x.ControlNo)
                    .Select(x => x.ControlNo)
                    .FirstOrDefaultAsync();

                int nextSequence = 1;
                if (!string.IsNullOrEmpty(lastControlNo))
                {
                    var lastSeq = lastControlNo.Substring(prefix.Length);
                    int.TryParse(lastSeq, out nextSequence);
                    nextSequence++;
                }

                importData.ControlNo = prefix + nextSequence.ToString("D4");
                importData.DateImported = DateTime.UtcNow;

                // ── Default checkbox values ──────────────────────────────────
                importData.RenewalAdditionalMold ??= "NO";
                importData.NewToolingLocalization ??= "NO";
                importData.TransferTooling ??= "NO";
                importData.ChangeMaterial ??= "NO";
                importData.NewModel ??= "NO";
                importData.NonConcurrent ??= "NO";
                importData.SupplierChangeLocalization ??= "NO";
                importData.Other4M ??= "NO";
                importData.MultipleProcurementLocalization ??= "NO";

                // ── Determine initial CurrentProcess per selected activity ───
                // Each activity has its own starting process.
                // If multiple are checked, we create one ActivityCurrentProcess per activity.
                var activityProcessEntries = BuildActivityCurrentProcesses(importData);
                foreach (var acp in activityProcessEntries)
                    _dbContext.ActivityCurrentProcesses.Add(acp);

                // ── Transaction logs ─────────────────────────────────────────
                var logRows = BuildTransactionLogRows(importData);
                if (logRows.Any())
                    _dbContext.TransactionLogs.AddRange(logRows);

                _dbContext.ImportDatas.Add(importData);
                await _dbContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Saved successfully! Control No: {importData.ControlNo}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        // ================================================================
        // HELPER: builds one ActivityCurrentProcess per selected activity,
        // each with the correct starting process for that activity.
        // ================================================================
        private List<ActivityCurrentProcess> BuildActivityCurrentProcesses(ImportData data)
        {
            var entries = new List<ActivityCurrentProcess>();

            if (data.RenewalAdditionalMold?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Tooling Quotation Request~Approval", "Renewal"));

            if (data.NewToolingLocalization?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Tooling PO Issued Date", "Localization"));

            if (data.SupplierChangeLocalization?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Mold LOA", "SupplierChange"));

            if (data.MultipleProcurementLocalization?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Mold LOA", "MultipleProcurement"));

            if (data.TransferTooling?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Tooling Quotation Request~Approval", "TransferTooling"));

            if (data.ChangeMaterial?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Material LOA", "ChangeMaterial"));

            if (data.NewModel?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Tooling Quotation Request~Approval", "NewModel"));

            if (data.NonConcurrent?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Tooling Quotation Request~Approval", "NonConcurrent"));

            if (data.Other4M?.Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                entries.Add(Make(data.ControlNo, "Test Run meeting date", "Other4M"));

            return entries;
        }

        private static ActivityCurrentProcess Make(string controlNo, string process, string activityType) =>
     new ActivityCurrentProcess
     {
         ControlNumber = controlNo,
         CurrentProcess = process,
         UpdateAt = DateTime.UtcNow,
         ActivityType = activityType
     };

        // ================================================================
        // HELPER: builds TransactionLog rows per selected activity
        // ================================================================
        private List<TransactionLogs> BuildTransactionLogRows(ImportData data)
        {
            var activityMap = new List<(string name, Func<ImportData, string> selector, string initialProcess)>
    {
        ("Renewal / Additional Mold",           x => x.RenewalAdditionalMold,           "Tooling Quotation Request~Approval"),
        ("New Tooling / Localization",          x => x.NewToolingLocalization,          "Tooling PO Issued Date"),
        ("Transfer Tooling",                    x => x.TransferTooling,                 "Tooling Quotation Request~Approval"),
        ("Change Material",                     x => x.ChangeMaterial,                  "Material LOA"),
        ("New Model",                           x => x.NewModel,                        "Tooling Quotation Request~Approval"),
        ("Non-Concurrent",                      x => x.NonConcurrent,                   "Tooling Quotation Request~Approval"),
        ("Supplier Change / Localization",      x => x.SupplierChangeLocalization,      "Mold LOA"),
        ("Other 4M",                            x => x.Other4M,                         "Testing Run meeting date"),
        ("Multiple Procurement / Localization", x => x.MultipleProcurementLocalization, "Mold LOA"),
    };

            var rows = new List<TransactionLogs>();

            foreach (var (activityName, selector, initialProcess) in activityMap)
            {
                if (selector(data)?.Equals("YES", StringComparison.OrdinalIgnoreCase) != true)
                    continue;

                rows.Add(new TransactionLogs
                {
                    TransactionNumber = data.ControlNo,
                    PartName = data.PartName,
                    Supplier = data.Supplier,
                    Model = data.Model,
                    Activity = activityName,
                    Source = "Import File",
                    PIC = data.Section ?? "SYSTEM",
                    StartDate = data.DateImported,
                    EndDate = null,
                    ReceivedDate = null,
                    InputDate = data.DateImported,
                    CurrentProcess = initialProcess,
                    Status = "In Progress",
                    Remarks = string.Empty
                });
            }

            return rows;
        }

        public IActionResult DownloadImportTemplate()
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "templates",
                "ImportData_Batch Entry Template.xlsx");

            return PhysicalFile(
                path,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ImportData_Batch Entry Template.xlsx");
        }

        private async Task<int> GetNextControlSequenceAsync(string prefix)
        {
            var lastControlNo = await _dbContext.ImportDatas
                .Where(x => x.ControlNo.StartsWith(prefix))
                .OrderByDescending(x => x.ControlNo)
                .Select(x => x.ControlNo)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(lastControlNo)) return 1;

            var lastSeqStr = lastControlNo.Substring(prefix.Length);
            return int.TryParse(lastSeqStr, out int lastSeq) ? lastSeq + 1 : 1;
        }

        [HttpPost]
        public async Task<IActionResult> PreviewExcelTemplate(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
                return BadRequest("No file uploaded");

            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var prefix = $"IMP-DATA-{today}-";
            int sequence = await GetNextControlSequenceAsync(prefix);
            var list = new List<ImportData>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = excelFile.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);

            int rowIndex = 0;

            while (reader.Read())
            {
                rowIndex++;
                if (rowIndex < 5) continue; // Skip header rows

                var motherMoldCode = reader.GetValue(0)?.ToString()?.Trim();
                var childPartcode = reader.GetValue(1)?.ToString()?.Trim();
                var partName = reader.GetValue(2)?.ToString()?.Trim();
                var model = reader.GetValue(3)?.ToString()?.Trim();
                var supplier = reader.GetValue(4)?.ToString()?.Trim();
                var moldMaker = reader.GetValue(5)?.ToString()?.Trim();
                var supplierMoldNo = reader.GetValue(6)?.ToString()?.Trim();
                var biphMoldNo = reader.GetValue(7)?.ToString()?.Trim();
                var toolingManagement = reader.GetValue(8)?.ToString()?.Trim();
                var toolingType = reader.GetValue(9)?.ToString()?.Trim();       // ← add
                var toolingCategory = reader.GetValue(10)?.ToString()?.Trim();  // ← add
                var renewalAdditionalMold = reader.GetValue(11)?.ToString()?.Trim();
                var newTooling = reader.GetValue(12)?.ToString()?.Trim();
                var supplierChange = reader.GetValue(13)?.ToString()?.Trim();
                var multipleProcurement = reader.GetValue(14)?.ToString()?.Trim();
                var transferTooling = reader.GetValue(15)?.ToString()?.Trim();
                var changeMaterial = reader.GetValue(16)?.ToString()?.Trim();
                var newModel = reader.GetValue(17)?.ToString()?.Trim();
                var nonConcurrent = reader.GetValue(18)?.ToString()?.Trim();
                var other4m = reader.GetValue(19)?.ToString()?.Trim();
                var reasonOfChange = reader.GetValue(20)?.ToString()?.Trim();

                // Skip blank rows
                if (string.IsNullOrEmpty(motherMoldCode) && string.IsNullOrEmpty(partName) &&
                    string.IsNullOrEmpty(supplier) && string.IsNullOrEmpty(reasonOfChange))
                    continue;

                bool IsYes(string val) =>
                    val?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true;

                var item = new ImportData
                {
                    ControlNo = prefix + sequence.ToString("D4"),
                    MotherMoldCode = motherMoldCode,
                    ChildPartcode = childPartcode,
                    PartName = partName,
                    Model = model,
                    Supplier = supplier,
                    MoldMaker = moldMaker,
                    SupplierMoldNo = supplierMoldNo,
                    BIPHMoldNo = biphMoldNo,
                    ToolingManagement = toolingManagement,
                    ToolingType = string.IsNullOrWhiteSpace(toolingType) ? "N/A" : toolingType,
                    ToolingCategory = string.IsNullOrWhiteSpace(toolingCategory) ? "N/A" : toolingCategory,
                    RenewalAdditionalMold = IsYes(renewalAdditionalMold) ? "YES" : "NO",
                    NewToolingLocalization = IsYes(newTooling) ? "YES" : "NO",
                    SupplierChangeLocalization = IsYes(supplierChange) ? "YES" : "NO",
                    MultipleProcurementLocalization = IsYes(multipleProcurement) ? "YES" : "NO",
                    TransferTooling = IsYes(transferTooling) ? "YES" : "NO",
                    ChangeMaterial = IsYes(changeMaterial) ? "YES" : "NO",
                    NewModel = IsYes(newModel) ? "YES" : "NO",
                    NonConcurrent = IsYes(nonConcurrent) ? "YES" : "NO",
                    Other4M = IsYes(other4m) ? "YES" : "NO",
                    ReasonOfChange = reasonOfChange,
                    DateImported = DateTime.UtcNow
                };

                list.Add(item);
                sequence++;
            }

            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcelBatch([FromBody] List<ImportData> importList)
        {
            if (importList == null || !importList.Any())
                return Json(new { success = false, message = "No data received for import." });

            try
            {
                var section = User.FindFirst("Section")?.Value;

                if (string.IsNullOrEmpty(section))
                    return Json(new { success = false, message = "User section not found. Please re-login." });

                foreach (var item in importList)
                {
                    item.Section = section;

                    item.RenewalAdditionalMold ??= "NO";
                    item.NewToolingLocalization ??= "NO";
                    item.TransferTooling ??= "NO";
                    item.ChangeMaterial ??= "NO";
                    item.NewModel ??= "NO";
                    item.NonConcurrent ??= "NO";
                    item.SupplierChangeLocalization ??= "NO";
                    item.Other4M ??= "NO";
                    item.MultipleProcurementLocalization ??= "NO";
                
                    item.DateImported = DateTime.UtcNow;
                    // Correct initial CurrentProcess per activity
                    var acpEntries = BuildActivityCurrentProcesses(item);
                    foreach (var acp in acpEntries)
                        _dbContext.ActivityCurrentProcesses.Add(acp);

                    // Transaction logs
                    var logRows = BuildTransactionLogRows(item);
                    if (logRows.Any())
                        _dbContext.TransactionLogs.AddRange(logRows);
                }

                // Duplicate check
                var controlNos = importList.Select(x => x.ControlNo).ToList();
                var existingControls = await _dbContext.ImportDatas
                    .Where(x => controlNos.Contains(x.ControlNo))
                    .Select(x => x.ControlNo)
                    .ToListAsync();

                if (existingControls.Any())
                    return Json(new
                    {
                        success = false,
                        message = $"Duplicate Control No found: {string.Join(", ", existingControls)}"
                    });

                await _dbContext.ImportDatas.AddRangeAsync(importList);
                await _dbContext.SaveChangesAsync();

                return Json(new { success = true, message = $"{importList.Count} rows imported successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportData(DateTime start, DateTime end)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            start = DateTime.SpecifyKind(start.Date, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var data = await _dbContext.ImportDatas
                .Where(x => x.DateImported >= start && x.DateImported <= end)
                .ToListAsync();

            if (!data.Any())
                return BadRequest(new { success = false, message = $"No records found between {start:yyyy-MM-dd} and {end:yyyy-MM-dd}." });

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Exported Data");

            var headers = new[]
            {
                "Control No", "Mother Mold Code", "Child Partcode", "Part Name", "Model",
                "Supplier", "Mold Maker", "Supplier Mold No", "BIPH Mold No", "Tooling Management",
                "Activity", "Reason Of Change",
                "Renewal Additional Mold", "New Tooling / Localization",
                "Supplier Change / Localization", "Multiple Procurement / Localization",
                "Transfer Tooling", "Change Material", "New Model", "Non Concurrent",
                "Other 4M", "Date Imported"
            };

            for (int i = 0; i < headers.Length; i++)
                ws.Cells[1, i + 1].Value = headers[i];

            int row = 2;
            foreach (var d in data)
            {
                ws.Cells[row, 1].Value = d.ControlNo;
                ws.Cells[row, 2].Value = d.MotherMoldCode;
                ws.Cells[row, 3].Value = d.ChildPartcode;
                ws.Cells[row, 4].Value = d.PartName;
                ws.Cells[row, 5].Value = d.Model;
                ws.Cells[row, 6].Value = d.Supplier;
                ws.Cells[row, 7].Value = d.MoldMaker;
                ws.Cells[row, 8].Value = d.SupplierMoldNo;
                ws.Cells[row, 9].Value = d.BIPHMoldNo;
                ws.Cells[row, 10].Value = d.ToolingManagement;
                ws.Cells[row, 11].Value = d.Activity;
                ws.Cells[row, 12].Value = d.ReasonOfChange;
                ws.Cells[row, 13].Value = d.RenewalAdditionalMold;
                ws.Cells[row, 14].Value = d.NewToolingLocalization;
                ws.Cells[row, 15].Value = d.SupplierChangeLocalization;
                ws.Cells[row, 16].Value = d.MultipleProcurementLocalization;
                ws.Cells[row, 17].Value = d.TransferTooling;
                ws.Cells[row, 18].Value = d.ChangeMaterial;
                ws.Cells[row, 19].Value = d.NewModel;
                ws.Cells[row, 20].Value = d.NonConcurrent;
                ws.Cells[row, 21].Value = d.Other4M;
                ws.Cells[row, 22].Value = d.DateImported.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            var fileBytes = package.GetAsByteArray();
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ExportedData_{start:yyyyMMdd}_to_{end:yyyyMMdd}.xlsx");
        }
    }
}