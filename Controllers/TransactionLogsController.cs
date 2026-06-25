using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
using PartsControlSystem.ViewModels;
using PartsControlSystem.Helpers;

namespace PartsControlSystem.Controllers
{
    public class TransactionLogsController : Controller
    {
        private readonly PostgreAppDbContext _dbContext;

        public TransactionLogsController(PostgreAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> TransactionLogs()
        {
            var logs = await _dbContext.TransactionLogs
                 .GroupBy(x => new { x.TransactionNumber, x.Activity })
                 .Select(g => g.OrderByDescending(x => x.InputDate).First())
                 .ToListAsync();

            var latestProcesses = await _dbContext.ActivityCurrentProcesses
                .GroupBy(x => x.ControlNumber)
                .Select(g => g.OrderByDescending(x => x.UpdateAt).First())
                .ToListAsync();

            var leadTimes = await _dbContext.LeadTimes.ToListAsync();
            var newToolingMappings = await _dbContext.NewToolingProcessMappings.ToListAsync();
            var changeMaterialMappings = await _dbContext.ChangeMaterialProcessMappings.ToListAsync();
            var other4MMappings = await _dbContext.Other4MProcessMappings.ToListAsync();
            var today = DateTime.UtcNow;

            var viewModel = logs
                .OrderByDescending(x => x.InputDate)
                .Select(x =>
                {
                    var latestProcess = latestProcesses
                        .FirstOrDefault(p => p.ControlNumber == x.TransactionNumber);

                    string actualCurrentProcess = latestProcess?.CurrentProcess ?? x.CurrentProcess;
                    bool isCompleted = IsCompleted(x.Activity, actualCurrentProcess);

                    string resolvedStatus = x.Status == "Deleted"
                        ? "Deleted"
                        : ActivityComputationHelper.ResolveTransactionLogStatus(
                            isCompleted, actualCurrentProcess, x.InputDate, x.Activity,
                            leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today);

                    return new TransactionLogViewModel
                    {
                        TransactionNumber = x.TransactionNumber,
                        PartName = x.PartName,
                        Supplier = x.Supplier,
                        Model = x.Model,
                        Activity = x.Activity,
                        Source = x.Source,
                        PIC = x.PIC,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        ReceivedDate = x.ReceivedDate,
                        InputDate = x.InputDate,
                        CurrentProcess = x.CurrentProcess,
                        Status = resolvedStatus,
                        Remarks = x.Remarks
                    };
                })
                .ToList();

            return View("TransactionLogs", viewModel);
        }

        // =====================================================================
        // DOWNLOAD — ClosedXML
        // =====================================================================
        [HttpGet]
        public async Task<IActionResult> Download(
            string? searchField,
            string? searchValue,
            string? startDate,
            string? endDate)
        {
            var logs = await _dbContext.TransactionLogs
                .GroupBy(x => new { x.TransactionNumber, x.Activity })
                .Select(g => g.OrderByDescending(x => x.InputDate).First())
                .ToListAsync();

            var latestProcesses = await _dbContext.ActivityCurrentProcesses
                .GroupBy(x => x.ControlNumber)
                .Select(g => g.OrderByDescending(x => x.UpdateAt).First())
                .ToListAsync();

            var leadTimes = await _dbContext.LeadTimes.ToListAsync();
            var newToolingMappings = await _dbContext.NewToolingProcessMappings.ToListAsync();
            var changeMaterialMappings = await _dbContext.ChangeMaterialProcessMappings.ToListAsync();
            var other4MMappings = await _dbContext.Other4MProcessMappings.ToListAsync();
            var today = DateTime.UtcNow;

            var rows = logs
                .OrderByDescending(x => x.InputDate)
                .Select(x =>
                {
                    var latestProcess = latestProcesses
                        .FirstOrDefault(p => p.ControlNumber == x.TransactionNumber);
                    string actualCurrentProcess = latestProcess?.CurrentProcess ?? x.CurrentProcess;
                    bool isCompleted = IsCompleted(x.Activity, actualCurrentProcess);

                    string resolvedStatus = x.Status == "Deleted"
                        ? "Deleted"
                        : ActivityComputationHelper.ResolveTransactionLogStatus(
                            isCompleted, actualCurrentProcess, x.InputDate, x.Activity,
                            leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today);

                    return new TransactionLogViewModel
                    {
                        TransactionNumber = x.TransactionNumber,
                        PartName = x.PartName,
                        Supplier = x.Supplier,
                        Model = x.Model,
                        Activity = x.Activity,
                        Source = x.Source,
                        PIC = x.PIC,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        ReceivedDate = x.ReceivedDate,
                        InputDate = x.InputDate,
                        CurrentProcess = x.CurrentProcess,
                        Status = resolvedStatus,
                        Remarks = x.Remarks
                    };
                })
                .ToList();

            // ── Apply search filter ────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(searchValue) && !string.IsNullOrWhiteSpace(searchField))
            {
                var val = searchValue.Trim().ToLower();
                rows = searchField switch
                {
                    "0" => rows.Where(r => (r.TransactionNumber ?? "").ToLower().Contains(val)).ToList(),
                    "1" => rows.Where(r => (r.PartName ?? "").ToLower().Contains(val)).ToList(),
                    "2" => rows.Where(r => (r.Supplier ?? "").ToLower().Contains(val)).ToList(),
                    "3" => rows.Where(r => (r.Model ?? "").ToLower().Contains(val)).ToList(),
                    "4" => rows.Where(r => (r.Activity ?? "").ToLower().Contains(val)).ToList(),
                    "6" => rows.Where(r => (r.PIC ?? "").ToLower().Contains(val)).ToList(),
                    _ => rows
                };
            }

            // ── Apply date filter (on StartDate, column index 7) ──────────
            if (DateTime.TryParse(startDate, out var sd))
                rows = rows.Where(r => r.StartDate.HasValue && r.StartDate.Value.ToLocalTime().Date >= sd.Date).ToList();
            if (DateTime.TryParse(endDate, out var ed))
                rows = rows.Where(r => r.StartDate.HasValue && r.StartDate.Value.ToLocalTime().Date <= ed.Date).ToList();

            // ── Build XLSX ─────────────────────────────────────────────────
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Transaction Logs");

            var headers = new[]
            {
                "Transaction No", "Part Name", "Supplier", "Model",
                "Activity", "Source", "PIC", "Start Date", "End Date",
                "Received Date", "Input Date", "Current Process", "Status", "Remarks"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Font.FontSize = 10;
                cell.Style.Font.FontName = "Arial";
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1565c0");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                cell.Style.Alignment.WrapText = true;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.OutsideBorderColor = XLColor.Black;
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                int rowNum = i + 2;
                bool isAlt = i % 2 == 1;
                var altColor = XLColor.FromHtml("#f0f4f8");

                void WriteCell(int col, string val, bool center = false)
                {
                    var cell = ws.Cell(rowNum, col);
                    cell.Value = val ?? "";
                    cell.Style.Font.FontSize = 10;
                    cell.Style.Font.FontName = "Arial";
                    cell.Style.Alignment.Horizontal = center
                        ? XLAlignmentHorizontalValues.Center
                        : XLAlignmentHorizontalValues.Left;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.Alignment.WrapText = true;
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.Black;
                    if (isAlt)
                        cell.Style.Fill.BackgroundColor = altColor;
                }

                WriteCell(1, r.TransactionNumber ?? "");
                WriteCell(2, r.PartName ?? "");
                WriteCell(3, r.Supplier ?? "");
                WriteCell(4, r.Model ?? "");
                WriteCell(5, r.Activity ?? "");
                WriteCell(6, r.Source ?? "");
                WriteCell(7, r.PIC ?? "");
                WriteCell(8, r.StartDate.HasValue ? r.StartDate.Value.ToLocalTime().ToString("MM/dd/yyyy") : "", true);
                WriteCell(9, r.EndDate.HasValue ? r.EndDate.Value.ToLocalTime().ToString("MM/dd/yyyy") : "", true);
                WriteCell(10, r.ReceivedDate.HasValue ? r.ReceivedDate.Value.ToLocalTime().ToString("MM/dd/yyyy") : "", true);
                WriteCell(11, r.InputDate.HasValue ? r.InputDate.Value.ToLocalTime().ToString("MM/dd/yyyy HH:mm") : "", true);
                WriteCell(12, r.CurrentProcess ?? "");
                WriteCell(14, r.Remarks ?? "");

                // ── Status cell — color coded ──────────────────────────────
                var statusCell = ws.Cell(rowNum, 13);
                statusCell.Value = r.Status ?? "";
                statusCell.Style.Font.Bold = true;
                statusCell.Style.Font.FontSize = 10;
                statusCell.Style.Font.FontName = "Arial";
                statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                statusCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                statusCell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                statusCell.Style.Border.OutsideBorderColor = XLColor.Black;

                switch (r.Status)
                {
                    case "Completed":
                        statusCell.Style.Font.FontColor = XLColor.FromHtml("#166534");
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#dcfce7");
                        break;
                    case "Delay":
                        statusCell.Style.Font.FontColor = XLColor.FromHtml("#9a3412");
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#ffedd5");
                        break;
                    case "Deleted":
                        statusCell.Style.Font.FontColor = XLColor.FromHtml("#991b1b");
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#fee2e2");
                        break;
                    default: // In Progress
                        statusCell.Style.Font.FontColor = XLColor.FromHtml("#92400e");
                        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#fef9c3");
                        break;
                }

                ws.Row(rowNum).Height = 18;
            }

            ws.Columns().AdjustToContents();
            foreach (var col in ws.ColumnsUsed())
            {
                if (col.Width < 12) col.Width = 12;
                if (col.Width > 60) col.Width = 60;
            }

            ws.Row(1).Height = 28;
            ws.SheetView.FreezeRows(1);
            if (ws.RangeUsed() != null) ws.RangeUsed().SetAutoFilter();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"TransactionLogs_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            );
        }

        // =====================================================================
        // GET HISTORY
        // =====================================================================
        [HttpGet]
        public async Task<IActionResult> GetHistory(string transactionNumber, string activity)
        {
            if (string.IsNullOrWhiteSpace(transactionNumber))
                return BadRequest(new { success = false, message = "Transaction number is required." });

            var query = _dbContext.TransactionLogs
                .Where(x => x.TransactionNumber == transactionNumber);

            if (!string.IsNullOrWhiteSpace(activity))
                query = query.Where(x => x.Activity == activity);

            var history = await query
                .OrderByDescending(x => x.InputDate)
                .ToListAsync();

            var latestProcess = await _dbContext.ActivityCurrentProcesses
                .Where(x => x.ControlNumber == transactionNumber)
                .OrderByDescending(x => x.UpdateAt)
                .FirstOrDefaultAsync();

            string actualCurrentProcess = latestProcess?.CurrentProcess ?? string.Empty;

            var leadTimes = await _dbContext.LeadTimes.ToListAsync();
            var newToolingMappings = await _dbContext.NewToolingProcessMappings.ToListAsync();
            var changeMaterialMappings = await _dbContext.ChangeMaterialProcessMappings.ToListAsync();
            var other4MMappings = await _dbContext.Other4MProcessMappings.ToListAsync();
            var today = DateTime.UtcNow;

            var result = history.Select((row, index) =>
            {
                string status;
                if (row.Status == "Deleted")
                {
                    status = "Deleted";
                }
                else if (index == 0)
                {
                    bool isCompleted = IsCompleted(row.Activity, actualCurrentProcess);
                    status = ActivityComputationHelper.ResolveTransactionLogStatus(
                        isCompleted, actualCurrentProcess, row.InputDate, row.Activity,
                        leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today);
                }
                else
                {
                    status = "Completed"; // historical/superseded rows are always treated as done
                }

                return new
                {
                    row.TransactionNumber,
                    row.PartName,
                    row.Supplier,
                    row.Model,
                    row.Activity,
                    row.Source,
                    row.PIC,

                    StartDate = row.StartDate.HasValue
                        ? row.StartDate.Value.ToLocalTime().ToString("MM/dd/yyyy")
                        : "—",

                    EndDate = row.EndDate.HasValue
                        ? row.EndDate.Value.ToLocalTime().ToString("MM/dd/yyyy")
                        : "—",

                    InputDate = row.InputDate.HasValue
                        ? row.InputDate.Value.ToLocalTime().ToString("MM/dd/yyyy HH:mm")
                        : "—",

                    row.CurrentProcess,

                    Status = status,

                    row.Remarks
                };
            }).ToList();

            return Json(result);
        }

        // =====================================================================
        // IS COMPLETED
        // =====================================================================
        private static bool IsCompleted(string activity, string currentProcess)
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
            {
                return currentProcess == "Completed";
            }

            return currentProcess == "Completed";
        }

        // =====================================================================
        // BACKFILL
        // =====================================================================
        [HttpPost]
        public async Task<IActionResult> BackfillTransactionLogs()
        {
            try
            {
                var importDataList = await _dbContext.ImportDatas.ToListAsync();

                var quotations = await _dbContext.ToolingQuotationRequestApproval.ToListAsync();
                var requestOrders = await _dbContext.MP2ToolingRequestOrder.ToListAsync();
                var poIssuances = await _dbContext.MP2ToolingPoIssuance.ToListAsync();
                var dfmApprovals = await _dbContext.SQCDFMQCDApprovals.ToListAsync();
                var fabrications = await _dbContext.MP2ToolingFabrications.ToListAsync();
                var transfers = await _dbContext.MP2ToolingTransfers.ToListAsync();
                var katakenSubs = await _dbContext.IQCKatakenSubmissions.ToListAsync();
                var katakenFinish = await _dbContext.IQCKatakenFinish.ToListAsync();
                var deEvals = await _dbContext.DEEvaluation.ToListAsync();
                var qaEvals = await _dbContext.QASpecialEvaluations.ToListAsync();
                var testRuns = await _dbContext.IQCTestRuns.ToListAsync();

                var existingKeys = await _dbContext.TransactionLogs
                    .Select(x => new { x.TransactionNumber, x.Source, x.CurrentProcess })
                    .ToListAsync();

                var newRows = new List<TransactionLogs>();

                foreach (var imp in importDataList)
                {
                    var activityMap = new Dictionary<string, string>
                    {
                        ["Renewal / Additional Mold"] = imp.RenewalAdditionalMold,
                        ["New Tooling / Localization"] = imp.NewToolingLocalization,
                        ["Transfer Tooling"] = imp.TransferTooling,
                        ["Change Material"] = imp.ChangeMaterial,
                        ["New Model"] = imp.NewModel,
                        ["Non-Concurrent"] = imp.NonConcurrent,
                        ["Supplier Change / Localization"] = imp.SupplierChangeLocalization,
                        ["Other 4M"] = imp.Other4M,
                        ["Multiple Procurement / Localization"] = imp.MultipleProcurementLocalization,
                    };

                    foreach (var (activityName, flag) in activityMap)
                    {
                        if (!string.Equals(flag, "YES", StringComparison.OrdinalIgnoreCase))
                            continue;

                        bool alreadyExists = existingKeys.Any(x =>
                            x.TransactionNumber == imp.ControlNo &&
                            x.Source == "Import File" &&
                            x.CurrentProcess == "Tooling Quotation Request~Approval");

                        if (alreadyExists) continue;

                        newRows.Add(new TransactionLogs
                        {
                            TransactionNumber = imp.ControlNo,
                            PartName = imp.PartName,
                            Supplier = imp.Supplier,
                            Model = imp.Model,
                            Activity = activityName,
                            Source = "Import File",
                            PIC = imp.Section ?? "SYSTEM",
                            StartDate = imp.DateImported,
                            EndDate = null,
                            ReceivedDate = null,
                            InputDate = imp.DateImported,
                            CurrentProcess = "Tooling Quotation Request~Approval",
                            Status = "In Progress",
                            Remarks = string.Empty
                        });
                    }
                }

                bool IsDupe(string controlNo, string source, string process) =>
                    existingKeys.Any(x =>
                        x.TransactionNumber == controlNo &&
                        x.Source == source &&
                        x.CurrentProcess == process);

                TransactionLogs MakeRow(
                    string controlNo,
                    string currentProcess,
                    string nextProcess,
                    string inputBy,
                    string remarks,
                    DateTime? inputDate,
                    DateTime? endDate)
                {
                    var imp = importDataList.FirstOrDefault(x => x.ControlNo == controlNo);
                    if (imp == null) return null;

                    string act = DetermineActivity(imp);
                    return new TransactionLogs
                    {
                        TransactionNumber = controlNo,
                        PartName = imp.PartName,
                        Supplier = imp.Supplier,
                        Model = imp.Model,
                        Activity = act,
                        Source = "Update Activity",
                        PIC = inputBy ?? "SYSTEM",
                        StartDate = imp.DateImported,
                        EndDate = endDate,
                        ReceivedDate = endDate,
                        InputDate = inputDate,
                        CurrentProcess = currentProcess,
                        Status = IsCompleted(act, nextProcess)
                                                ? "Completed"
                                                : "In Progress",
                        Remarks = remarks ?? string.Empty
                    };
                }

                foreach (var r in quotations)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling Quotation Request~Approval")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling Quotation Request~Approval", "Tooling Request-Order", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in requestOrders)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling Request-Order")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling Request-Order", "Tooling PO Issuance", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in poIssuances)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling PO Issuance")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling PO Issuance", "DFM/QCD Approval", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in dfmApprovals)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "DFM/QCD Approval")) continue;
                    var row = MakeRow(r.ControlNumber, "DFM/QCD Approval", "Tooling Fabrication", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in fabrications)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling Fabrication")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling Fabrication", "Tooling Transfer (Arrival in PH)", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in transfers)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling Transfer (Arrival in PH)")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling Transfer (Arrival in PH)", "Kataken Submission (Local Trial)", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in katakenSubs)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Kataken Submission (Local Trial)")) continue;
                    var row = MakeRow(r.ControlNumber, "Kataken Submission (Local Trial)", "Kataken Finish (Local Trial)", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in katakenFinish)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Kataken Finish (Local Trial)")) continue;
                    var row = MakeRow(r.ControlNumber, "Kataken Finish (Local Trial)", "DE Evaluation", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in deEvals)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "DE Evaluation")) continue;
                    var row = MakeRow(r.ControlNumber, "DE Evaluation", "QA Special Evaluation", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in qaEvals)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "QA Special Evaluation")) continue;
                    var row = MakeRow(r.ControlNumber, "QA Special Evaluation", "Test Run", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in testRuns)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Test Run")) continue;
                    var row = MakeRow(r.ControlNumber, "Test Run", "MP2-PDC", r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                if (newRows.Any())
                {
                    await _dbContext.TransactionLogs.AddRangeAsync(newRows);
                    await _dbContext.SaveChangesAsync();
                }

                return Ok(new { success = true, message = $"Backfill complete. {newRows.Count} rows inserted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }

        private string DetermineActivity(PartsControlSystem.Models.ImportData data)
        {
            if (data.RenewalAdditionalMold == "YES") return "Renewal / Additional Mold";
            if (data.NewToolingLocalization == "YES") return "New Tooling / Localization";
            if (data.TransferTooling == "YES") return "Transfer Tooling";
            if (data.ChangeMaterial == "YES") return "Change Material";
            if (data.NewModel == "YES") return "New Model";
            if (data.NonConcurrent == "YES") return "Non-Concurrent";
            if (data.SupplierChangeLocalization == "YES") return "Supplier Change / Localization";
            if (data.Other4M == "YES") return "Other 4M";
            if (data.MultipleProcurementLocalization == "YES") return "Multiple Procurement / Localization";
            return "Unknown";
        }

        // =====================================================================
        // MISSING LOGS REPORT
        // =====================================================================
        [HttpGet]
        public async Task<IActionResult> MissingLogsReport()
        {
            var importList = await _dbContext.ImportDatas.ToListAsync();

            var loggedPairs = await _dbContext.TransactionLogs
                .Select(x => new { x.TransactionNumber, x.Activity })
                .Distinct()
                .ToListAsync();

            var loggedSet = loggedPairs
                .Select(x => (x.TransactionNumber, x.Activity))
                .ToHashSet();

            var flaggedPairs = new List<(string ControlNo, string Activity, string Section, DateTime? DateImported)>();

            foreach (var imp in importList)
            {
                var activityMap = new Dictionary<string, string>
                {
                    ["Renewal / Additional Mold"] = imp.RenewalAdditionalMold,
                    ["New Tooling / Localization"] = imp.NewToolingLocalization,
                    ["Transfer Tooling"] = imp.TransferTooling,
                    ["Change Material"] = imp.ChangeMaterial,
                    ["New Model"] = imp.NewModel,
                    ["Non-Concurrent"] = imp.NonConcurrent,
                    ["Supplier Change / Localization"] = imp.SupplierChangeLocalization,
                    ["Other 4M"] = imp.Other4M,
                    ["Multiple Procurement / Localization"] = imp.MultipleProcurementLocalization,
                };

                foreach (var (activityName, flag) in activityMap)
                {
                    if (string.Equals(flag, "YES", StringComparison.OrdinalIgnoreCase))
                        flaggedPairs.Add((imp.ControlNo, activityName, imp.Section, imp.DateImported));
                }
            }

            var missing = flaggedPairs
                .Where(f => !loggedSet.Contains((f.ControlNo, f.Activity)))
                .OrderBy(f => f.ControlNo)
                .ThenBy(f => f.Activity)
                .Select(f => new
                {
                    ControlNo = f.ControlNo,
                    Activity = f.Activity,
                    Section = f.Section,
                    DateImported = f.DateImported.HasValue
                        ? f.DateImported.Value.ToLocalTime().ToString("MM/dd/yyyy HH:mm")
                        : "—"
                })
                .ToList();

            var flaggedSet = flaggedPairs
                .Select(f => (f.ControlNo, f.Activity))
                .ToHashSet();

            var orphanLoggedPairs = loggedPairs
                .Where(l => !flaggedSet.Contains((l.TransactionNumber, l.Activity)))
                .OrderBy(l => l.TransactionNumber)
                .ThenBy(l => l.Activity)
                .ToList();

            return Json(new
            {
                totalFlagged = flaggedPairs.Count,
                totalLoggedPairs = loggedSet.Count,
                missingCount = missing.Count,
                missing,
                orphanLoggedCount = orphanLoggedPairs.Count,
                orphanLoggedPairs
            });
        }
    }
}