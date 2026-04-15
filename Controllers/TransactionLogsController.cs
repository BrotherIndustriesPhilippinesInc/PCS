//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using PartsControlSystem.Data;
//using PartsControlSystem.ViewModels;

//namespace PartsControlSystem.Controllers
//{
//    public class TransactionLogsController : Controller
//    {
//        private readonly PostgreAppDbContext _dbContext;

//        public TransactionLogsController(PostgreAppDbContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public async Task<IActionResult> TransactionLogs()
//        {
//            var latestProcesses = await _dbContext.ActivityCurrentProcesses
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.UpdateAt).First())
//                .ToListAsync();

//            var importDataList = await _dbContext.ImportDatas.ToListAsync();

//            var quotationRemarks = await _dbContext.ToolingQuotationRequestApproval
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var requestOrderRemarks = await _dbContext.MP2ToolingRequestOrder
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var poIssuanceRemarks = await _dbContext.MP2ToolingPoIssuance
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var dfmRemarks = await _dbContext.SQCDFMQCDApprovals
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var fabricationRemarks = await _dbContext.MP2ToolingFabrications
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var transferRemarks = await _dbContext.MP2ToolingTransfers
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var katakenSubRemarks = await _dbContext.IQCKatakenSubmissions
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var katakenFinishRemarks = await _dbContext.IQCKatakenFinish
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var deRemarks = await _dbContext.DEEvaluation
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var qaRemarks = await _dbContext.QASpecialEvaluations
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var testRunRemarks = await _dbContext.IQCTestRuns
//                .GroupBy(x => x.ControlNumber)
//                .Select(g => g.OrderByDescending(x => x.CreateDate).First())
//                .ToDictionaryAsync(x => x.ControlNumber, x => new { x.Remarks, x.InputBy, x.CreateDate, Source = "Update Activity" });

//            var logs = new List<TransactionLogViewModel>();

//            foreach (var imp in importDataList)
//            {
//                var latestProcess = latestProcesses
//                    .FirstOrDefault(x => x.ControlNumber == imp.ControlNo);

//                string activity = DetermineActivity(imp);
//                string currentProcess = latestProcess?.CurrentProcess ?? "Tooling Quotation Request~Approval";
//                string status = currentProcess == "MP2-PDC" ? "Completed" : "In Progress";

//                string remarks = string.Empty;
//                string inputBy = imp.Section ?? "System";
//                string source = "Import File";

//                var allProcessEntries = new List<(string Remarks, string InputBy, DateTime? CreateDate, string Source)>();

//                if (quotationRemarks.TryGetValue(imp.ControlNo, out var q))
//                    allProcessEntries.Add((q.Remarks, q.InputBy, q.CreateDate, q.Source));
//                if (requestOrderRemarks.TryGetValue(imp.ControlNo, out var ro))
//                    allProcessEntries.Add((ro.Remarks, ro.InputBy, ro.CreateDate, ro.Source));
//                if (poIssuanceRemarks.TryGetValue(imp.ControlNo, out var po))
//                    allProcessEntries.Add((po.Remarks, po.InputBy, po.CreateDate, po.Source));
//                if (dfmRemarks.TryGetValue(imp.ControlNo, out var dfm))
//                    allProcessEntries.Add((dfm.Remarks, dfm.InputBy, dfm.CreateDate, dfm.Source));
//                if (fabricationRemarks.TryGetValue(imp.ControlNo, out var fab))
//                    allProcessEntries.Add((fab.Remarks, fab.InputBy, fab.CreateDate, fab.Source));
//                if (transferRemarks.TryGetValue(imp.ControlNo, out var tr))
//                    allProcessEntries.Add((tr.Remarks, tr.InputBy, tr.CreateDate, tr.Source));
//                if (katakenSubRemarks.TryGetValue(imp.ControlNo, out var ks))
//                    allProcessEntries.Add((ks.Remarks, ks.InputBy, ks.CreateDate, ks.Source));
//                if (katakenFinishRemarks.TryGetValue(imp.ControlNo, out var kf))
//                    allProcessEntries.Add((kf.Remarks, kf.InputBy, kf.CreateDate, kf.Source));
//                if (deRemarks.TryGetValue(imp.ControlNo, out var de))
//                    allProcessEntries.Add((de.Remarks, de.InputBy, de.CreateDate, de.Source));
//                if (qaRemarks.TryGetValue(imp.ControlNo, out var qa))
//                    allProcessEntries.Add((qa.Remarks, qa.InputBy, qa.CreateDate, qa.Source));
//                if (testRunRemarks.TryGetValue(imp.ControlNo, out var tr2))
//                    allProcessEntries.Add((tr2.Remarks, tr2.InputBy, tr2.CreateDate, tr2.Source));

//                var latestEntry = allProcessEntries
//                    .OrderByDescending(x => x.CreateDate)
//                    .FirstOrDefault();

//                if (latestEntry != default)
//                {
//                    remarks = latestEntry.Remarks;
//                    inputBy = latestEntry.InputBy;
//                    source = latestEntry.Source;
//                    // ✅ intentionally NOT reading latestEntry.CreateDate for InputDate
//                }

//                logs.Add(new TransactionLogViewModel
//                {
//                    TransactionNumber = imp.ControlNo,
//                    PartName = imp.PartName,
//                    Supplier = imp.Supplier,
//                    Model = imp.Model,
//                    Activity = activity,
//                    Source = source,
//                    PIC = inputBy,
//                    StartDate = imp.DateImported,         // when originally imported — never changes
//                    EndDate = latestProcess?.UpdateAt,  // latest pipeline movement
//                    ReceivedDate = latestProcess?.UpdateAt,  // same as EndDate
//                    InputDate = imp.DateImported,         // ✅ FIXED: always the original import date
//                    CurrentProcess = currentProcess,
//                    Status = status,
//                    Remarks = remarks
//                });
//            }

//            var sorted = logs
//                .OrderByDescending(x => x.EndDate ?? x.StartDate)
//                .ToList();

//            return View("TransactionLogs", sorted);
//        }

//        private string DetermineActivity(PartsControlSystem.Models.ImportData data)
//        {
//            if (data.RenewalAdditionalMold == "YES") return "Renewal / Additional Mold";
//            if (data.NewToolingLocalization == "YES") return "New Tooling / Localization";
//            if (data.TransferTooling == "YES") return "Transfer Tooling";
//            if (data.ChangeMaterial == "YES") return "Change Material";
//            if (data.NewModel == "YES") return "New Model";
//            if (data.NonConcurrent == "YES") return "Non-Concurrent";
//            if (data.SupplierChangeLocalization == "YES") return "Supplier Change / Localization";
//            if (data.Other4M == "YES") return "Other 4M";
//            return "Unknown";
//        }
//    }
//}



using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
using PartsControlSystem.ViewModels;

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
            // ── Get only the latest row per TransactionNumber for the main table ──
            var logs = await _dbContext.TransactionLogs
                .GroupBy(x => x.TransactionNumber)
                .Select(g => g.OrderByDescending(x => x.InputDate).First())
                .ToListAsync();

            var viewModel = logs
                .OrderByDescending(x => x.InputDate)
                .Select(x => new TransactionLogViewModel
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
                    Status = x.Status,
                    Remarks = x.Remarks
                })
                .ToList();

            return View("TransactionLogs", viewModel);
        }

        // ── Called by the history modal button via AJAX ──
        //[HttpGet]
        //public async Task<IActionResult> GetHistory(string transactionNumber)
        //{
        //    if (string.IsNullOrWhiteSpace(transactionNumber))
        //        return BadRequest(new { success = false, message = "Transaction number is required." });

        //    var history = await _dbContext.TransactionLogs
        //        .Where(x => x.TransactionNumber == transactionNumber)
        //        .OrderByDescending(x => x.InputDate)
        //        .Select(x => new
        //        {
        //            x.TransactionNumber,
        //            x.Activity,
        //            x.Source,
        //            x.PIC,
        //            StartDate = x.StartDate.HasValue ? x.StartDate.Value.ToLocalTime().ToString("MM/dd/yyyy") : "—",
        //            EndDate = x.EndDate.HasValue ? x.EndDate.Value.ToLocalTime().ToString("MM/dd/yyyy") : "—",
        //            ReceivedDate = x.ReceivedDate.HasValue ? x.ReceivedDate.Value.ToLocalTime().ToString("MM/dd/yyyy") : "—",
        //            InputDate = x.InputDate.HasValue ? x.InputDate.Value.ToLocalTime().ToString("MM/dd/yyyy HH:mm") : "—",
        //            x.CurrentProcess,
        //            x.Status,
        //            x.Remarks
        //        })
        //        .ToListAsync();

        //    return Json(history);
        //}

      [HttpGet]
public async Task<IActionResult> GetHistory(string transactionNumber)
{
    if (string.IsNullOrWhiteSpace(transactionNumber))
        return BadRequest(new { success = false, message = "Transaction number is required." });

    var history = await _dbContext.TransactionLogs
        .Where(x => x.TransactionNumber == transactionNumber)
        .OrderByDescending(x => x.InputDate)
        .ToListAsync();

    var result = history.Select((row, index) =>
    {
        string status;

        if (index == 0)
        {
            // latest row — completed only if it reached the final process
            status = row.CurrentProcess == "MP2-PDC" ? "Completed" : "In Progress";
        }
        else
        {
            // older rows — already moved to a newer step so they are completed
            status = "Completed";
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
            StartDate      = row.StartDate.HasValue  ? row.StartDate.Value.ToLocalTime().ToString("MM/dd/yyyy")       : "—",
            EndDate        = row.EndDate.HasValue    ? row.EndDate.Value.ToLocalTime().ToString("MM/dd/yyyy")         : "—",
            InputDate      = row.InputDate.HasValue  ? row.InputDate.Value.ToLocalTime().ToString("MM/dd/yyyy HH:mm") : "—",
            row.CurrentProcess,
            Status         = status,
            row.Remarks
        };
    }).ToList();

    return Json(result);
}
        [HttpPost]
        public async Task<IActionResult> BackfillTransactionLogs()
        {
            try
            {
                // ── 1. Load everything we need ──
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

                // ── 2. Get all ControlNos already in transaction_logs to avoid duplicates ──
                var existingKeys = await _dbContext.TransactionLogs
                    .Select(x => new { x.TransactionNumber, x.Source, x.CurrentProcess })
                    .ToListAsync();

                var newRows = new List<TransactionLogs>();

                // ── 3. Backfill import rows (one per YES activity flag) ──
                foreach (var imp in importDataList)
                {
                    string activity = DetermineActivity(imp);

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
                    };

                    foreach (var (activityName, flag) in activityMap)
                    {
                        if (!string.Equals(flag, "YES", StringComparison.OrdinalIgnoreCase))
                            continue;

                        // skip if already backfilled
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

                // ── 4. Backfill each process table ──

                // helper to avoid repeating the duplicate check
                bool IsDupe(string controlNo, string source, string process) =>
                    existingKeys.Any(x =>
                        x.TransactionNumber == controlNo &&
                        x.Source == source &&
                        x.CurrentProcess == process);

                // helper to find the matching import row
                TransactionLogs MakeRow(string controlNo, string nextProcess,
                    string inputBy, string remarks, DateTime? inputDate,
                    DateTime? endDate)
                {
                    var imp = importDataList.FirstOrDefault(x => x.ControlNo == controlNo);
                    if (imp == null) return null;

                    return new TransactionLogs
                    {
                        TransactionNumber = controlNo,
                        PartName = imp.PartName,
                        Supplier = imp.Supplier,
                        Model = imp.Model,
                        Activity = DetermineActivity(imp),
                        Source = "Update Activity",
                        PIC = inputBy ?? "SYSTEM",
                        StartDate = imp.DateImported,
                        EndDate = endDate,
                        ReceivedDate = endDate,
                        InputDate = inputDate,
                        CurrentProcess = nextProcess,
                        Status = nextProcess == "MP2-PDC" ? "Completed" : "In Progress",
                        Remarks = remarks ?? string.Empty
                    };
                }

                foreach (var r in quotations)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling Request-Order")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling Request-Order",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in requestOrders)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling PO Issuance")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling PO Issuance",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in poIssuances)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "DFM/QCD Approval")) continue;
                    var row = MakeRow(r.ControlNumber, "DFM/QCD Approval",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in dfmApprovals)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling Fabrication")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling Fabrication",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in fabrications)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Tooling Transfer (Arrival in PH)")) continue;
                    var row = MakeRow(r.ControlNumber, "Tooling Transfer (Arrival in PH)",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in transfers)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Kataken Submission (Local Trial)")) continue;
                    var row = MakeRow(r.ControlNumber, "Kataken Submission (Local Trial)",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in katakenSubs)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Kataken Finish (Local Trial)")) continue;
                    var row = MakeRow(r.ControlNumber, "Kataken Finish (Local Trial)",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in katakenFinish)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "DE Evaluation")) continue;
                    var row = MakeRow(r.ControlNumber, "DE Evaluation",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in deEvals)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "QA Special Evaluation")) continue;
                    var row = MakeRow(r.ControlNumber, "QA Special Evaluation",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in qaEvals)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "Test Run")) continue;
                    var row = MakeRow(r.ControlNumber, "Test Run",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                foreach (var r in testRuns)
                {
                    if (IsDupe(r.ControlNumber, "Update Activity", "MP2-PDC")) continue;
                    var row = MakeRow(r.ControlNumber, "MP2-PDC",
                        r.InputBy, r.Remarks, r.CreateDate, r.CreateDate);
                    if (row != null) newRows.Add(row);
                }

                // ── 5. Bulk insert ──
                if (newRows.Any())
                {
                    await _dbContext.TransactionLogs.AddRangeAsync(newRows);
                    await _dbContext.SaveChangesAsync();
                }

                return Ok(new
                {
                    success = true,
                    message = $"Backfill complete. {newRows.Count} rows inserted."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
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
            return "Unknown";

        }
    }
}