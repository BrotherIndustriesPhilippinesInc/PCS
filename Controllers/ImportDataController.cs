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

        // POST: AJAX save
        [HttpPost]
        public async Task<IActionResult> SaveImportData([FromBody] ImportData importData)
        {
            if (importData == null)
                return Json(new { success = false, message = "Invalid data." });

            try
            {
                // ================= GET SECTION FROM LOGGED-IN USER =================
                var section = User.FindFirst("Section")?.Value;

                if (string.IsNullOrEmpty(section))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Section not found. Please re-login."
                    });
                }

                importData.Section = section;

                // ================= GENERATE CONTROL NO =================
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

                // ================= DEFAULT CHECKBOX VALUES =================
                importData.RenewalAdditionalMold ??= "NO";
                importData.NewToolingLocalization ??= "NO";
                importData.TransferTooling ??= "NO";
                importData.ChangeMaterial ??= "NO";
                importData.NewModel ??= "NO";
                importData.NonConcurrent ??= "NO";
                importData.SupplierChangeLocalization ??= "NO";
                importData.Other4M ??= "NO";

                string currentProcessValue = "Tooling Quotation Request~Approval";

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = importData.ControlNo,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                //FOR TRANSACTION LOGS
                var logRows = BuildTransactionLogRows(importData, currentProcessValue);
                if (logRows.Any())
                    _dbContext.TransactionLogs.AddRange(logRows);
                //END FOR TRANSACTION LOGS
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
                return Json(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //Download template for batch import
        public IActionResult DownloadImportTemplate()
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "templates",
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

            if (string.IsNullOrEmpty(lastControlNo))
                return 1;

            var lastSeqStr = lastControlNo.Substring(prefix.Length);
            return int.TryParse(lastSeqStr, out int lastSeq)
                ? lastSeq + 1
                : 1;
        }

        //FOR TRANSACTION LOGS
        private List<TransactionLogs> BuildTransactionLogRows(ImportData data, string currentProcess)
        {
            var activityMap = new Dictionary<string, Func<ImportData, string>>
            {
                ["Renewal / Additional Mold"] = x => x.RenewalAdditionalMold,
                ["New Tooling / Localization"] = x => x.NewToolingLocalization,
                ["Transfer Tooling"] = x => x.TransferTooling,
                ["Change Material"] = x => x.ChangeMaterial,
                ["New Model"] = x => x.NewModel,
                ["Non-Concurrent"] = x => x.NonConcurrent,
                ["Supplier Change / Localization"] = x => x.SupplierChangeLocalization,
                ["Other 4M"] = x => x.Other4M,
            };

            var rows = new List<TransactionLogs>();

            foreach (var (activityName, selector) in activityMap)
            {
                if (!selector(data).Equals("YES", StringComparison.OrdinalIgnoreCase))
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
                    CurrentProcess = currentProcess,
                    Status = "In Progress",
                    Remarks = string.Empty
                });
            }

            return rows;
        }
        //END FOR TRANSACTION LOGS

        [HttpPost]
        public async Task<IActionResult> PreviewExcelTemplate(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
                return BadRequest("No file uploaded");

            // 🔹 Prepare ControlNo prefix
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var prefix = $"IMP-DATA-{today}-";

            int sequence = await GetNextControlSequenceAsync(prefix);

            var list = new List<ImportData>();

            // 🔹 Required for ExcelDataReader text encoding
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = excelFile.OpenReadStream();
            using var reader = ExcelReaderFactory.CreateReader(stream);

            int rowIndex = 0;

            while (reader.Read())
            {
                rowIndex++;

                // Skip rows 1–4 (headers)
                if (rowIndex < 5)
                    continue;

                // 🔹 Get values
                var motherMoldCode = reader.GetValue(0)?.ToString()?.Trim();
                var childPartcode = reader.GetValue(1)?.ToString()?.Trim();
                var partName = reader.GetValue(2)?.ToString()?.Trim();
                var model = reader.GetValue(3)?.ToString()?.Trim();
                var supplier = reader.GetValue(4)?.ToString()?.Trim();
                var moldMaker = reader.GetValue(5)?.ToString()?.Trim();
                var supplierMoldNo = reader.GetValue(6)?.ToString()?.Trim();
                var BiphMoldNo = reader.GetValue(7)?.ToString()?.Trim();
                var ToolingManagement = reader.GetValue(8)?.ToString()?.Trim();
                var RenewalAdditionalMold = reader.GetValue(9)?.ToString()?.Trim();
                var NewTooling = reader.GetValue(10)?.ToString()?.Trim();
                var TransferTooling = reader.GetValue(11)?.ToString()?.Trim();
                var ChangeMaterial = reader.GetValue(12)?.ToString()?.Trim();
                var NewModel = reader.GetValue(13)?.ToString()?.Trim();
                var NonConcurrent = reader.GetValue(14)?.ToString()?.Trim();
                var SupplierChange = reader.GetValue(15)?.ToString()?.Trim();
                var Other4m = reader.GetValue(16)?.ToString()?.Trim();
                var ReasonOfChange = reader.GetValue(17)?.ToString()?.Trim();

                // 🔹 Skip blank rows (all key fields empty)
                if (string.IsNullOrEmpty(motherMoldCode) &&
                    string.IsNullOrEmpty(childPartcode) &&
                    string.IsNullOrEmpty(partName) &&
                    string.IsNullOrEmpty(model) &&
                    string.IsNullOrEmpty(supplier) &&
                    string.IsNullOrEmpty(moldMaker) &&
                    string.IsNullOrEmpty(supplierMoldNo) &&
                    string.IsNullOrEmpty(BiphMoldNo) &&
                    string.IsNullOrEmpty(ToolingManagement) &&
                    string.IsNullOrEmpty(RenewalAdditionalMold) &&
                    string.IsNullOrEmpty(NewTooling) &&
                    string.IsNullOrEmpty(ChangeMaterial) &&
                    string.IsNullOrEmpty(NewModel) &&
                    string.IsNullOrEmpty(NonConcurrent) &&
                    string.IsNullOrEmpty(SupplierChange) &&
                    string.IsNullOrEmpty(Other4m) &&
                    string.IsNullOrEmpty(ReasonOfChange))
                {
                    continue;
                }

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
                    BIPHMoldNo = BiphMoldNo,
                    ToolingManagement =ToolingManagement,
                   
                    RenewalAdditionalMold = RenewalAdditionalMold?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true ? "YES" : "NO",
                    NewToolingLocalization = NewTooling?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true ? "YES" : "NO",
                    TransferTooling = TransferTooling?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true ? "YES" : "NO",
                    ChangeMaterial = ChangeMaterial?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true ? "YES" : "NO",
                    NewModel = NewModel?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true ? "YES" : "NO",
                    NonConcurrent = NonConcurrent?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true ? "YES" : "NO",
                    SupplierChangeLocalization = SupplierChange?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true ? "YES" : "NO",
                    Other4M = Other4m?.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase) == true ? "YES" : "NO",
                    ReasonOfChange = ReasonOfChange,

                    DateImported = DateTime.UtcNow
                };

                list.Add(item);
                sequence++;
            }


            return Json(list); // Preview only, no DB save yet
        }

        [HttpPost]
        public async Task<IActionResult> ImportExcelBatch([FromBody] List<ImportData> importList)
        {
            if (importList == null || !importList.Any())
                return Json(new { success = false, message = "No data received for import." });

            try
            {
                // ================= GET SECTION FROM LOGGED-IN USER =================
                var section = User.FindFirst("Section")?.Value;

                if (string.IsNullOrEmpty(section))
                {
                    return Json(new
                    {
                        success = false,
                        message = "User section not found. Please re-login."
                    });
                }

                // ================= STAMP SECTION ON ALL ROWS =================
                foreach (var item in importList)
                {
                    item.Section = section;

                    // Optional safety defaults (in case Excel sends nulls)
                    item.RenewalAdditionalMold ??= "NO";
                    item.NewToolingLocalization ??= "NO";
                    item.TransferTooling ??= "NO";
                    item.ChangeMaterial ??= "NO";
                    item.NewModel ??= "NO";
                    item.NonConcurrent ??= "NO";
                    item.SupplierChangeLocalization ??= "NO";
                    item.Other4M ??= "NO";

                    // Optional: ensure date consistency
                    item.DateImported = DateTime.UtcNow;

                    //FOR TRANSACTION LOGS
                    var logRows = BuildTransactionLogRows(item, "Tooling Quotation Request~Approval");
                    if (logRows.Any())
                        _dbContext.TransactionLogs.AddRange(logRows);
                    //END FOR TRANSACTION LOGS
                }

                // ================= DUPLICATE CHECK =================
                var controlNos = importList.Select(x => x.ControlNo).ToList();

                var existingControls = await _dbContext.ImportDatas
                    .Where(x => controlNos.Contains(x.ControlNo))
                    .Select(x => x.ControlNo)
                    .ToListAsync();

                if (existingControls.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Duplicate Control No found: {string.Join(", ", existingControls)}"
                    });
                }

                // ================= INSERT BATCH =================
                await _dbContext.ImportDatas.AddRangeAsync(importList);
                await _dbContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"{importList.Count} rows imported successfully!"
                });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);

                return Json(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportData(DateTime start, DateTime end)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Convert to UTC and include entire end day
            start = DateTime.SpecifyKind(start.Date, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            var data = await _dbContext.ImportDatas
                .Where(x => x.DateImported >= start && x.DateImported <= end)
                .ToListAsync();

            // ✅ Validate if no data found
            if (!data.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"No records found between {start:yyyy-MM-dd} and {end:yyyy-MM-dd}."
                });
            }

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Exported Data");

            // Add headers
            var headers = new[]
            {
                "Control No", "Mother Mold Code", "Child Partcode", "Part Name", "Model",
                "Supplier", "Mold Maker", "Supplier Mold No", "BIPH Mold No", "Tooling Management",
                "Activity", "Reason Of Change", "Renewal Additional Mold", "New Tooling Localization",
                "Transfer Tooling", "Change Material", "New Model", "Non Concurrent",
                "Supplier Change Localization", "Other 4M", "Date Imported"
            };

            for (int i = 0; i < headers.Length; i++)
                ws.Cells[1, i + 1].Value = headers[i];

            // Fill data
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
                ws.Cells[row, 15].Value = d.TransferTooling;
                ws.Cells[row, 16].Value = d.ChangeMaterial;
                ws.Cells[row, 17].Value = d.NewModel;
                ws.Cells[row, 18].Value = d.NonConcurrent;
                ws.Cells[row, 19].Value = d.SupplierChangeLocalization;
                ws.Cells[row, 20].Value = d.Other4M;
                ws.Cells[row, 21].Value = d.DateImported.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                row++;
            }

            var fileBytes = package.GetAsByteArray();
            return File(fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"ExportedData_{start:yyyyMMdd}_to_{end:yyyyMMdd}.xlsx");
        }




    }
}
