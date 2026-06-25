using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
using PartsControlSystem.Helpers;
using System.Text;

namespace PartsControlSystem.Controllers
{
    public class AllPartsDataController : Controller
    {
        private readonly PostgreAppDbContext _context;

        public AllPartsDataController(PostgreAppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AllPartsData()
        {
            var vm = await BuildViewModel();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetParts(string? activity, string? status)
        {
            var rows = await FetchRows(activity, status);
            return Json(rows);
        }

        [HttpGet]
        public async Task<IActionResult> Download(string? activity, string? status, string? month, string? year)
        {
            var rows = await FetchRows(activity, status);

            if (!string.IsNullOrEmpty(month) && month != "All" && int.TryParse(month, out int m))
                rows = rows.Where(r => r.InputDate.HasValue && r.InputDate.Value.ToLocalTime().Month == m).ToList();

            if (!string.IsNullOrEmpty(year) && year != "All" && int.TryParse(year, out int y))
                rows = rows.Where(r => r.InputDate.HasValue && r.InputDate.Value.ToLocalTime().Year == y).ToList();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.Worksheets.Add("All Parts Data");

            var headers = new[]
            {
                "Transaction Number", "Part Name", "Part Code", "Supplier",
                "Model", "Activity", "Input Date", "Status", "Current Process", "Remarks"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#1565c0");
                cell.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
                cell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                cell.Style.Border.BottomBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
                cell.Style.Border.BottomBorderColor = ClosedXML.Excel.XLColor.White;
            }

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                var row = ws.Row(i + 2);

                if (i % 2 == 1)
                {
                    row.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#f0f4f8");
                }

                ws.Cell(i + 2, 1).Value = r.TransactionNumber ?? "";
                ws.Cell(i + 2, 2).Value = r.PartName ?? "";
                ws.Cell(i + 2, 3).Value = r.PartCode ?? "";
                ws.Cell(i + 2, 4).Value = r.Supplier ?? "";
                ws.Cell(i + 2, 5).Value = r.Model ?? "";
                ws.Cell(i + 2, 6).Value = r.Activity ?? "";
                ws.Cell(i + 2, 7).Value = r.InputDate.HasValue
                    ? r.InputDate.Value.ToLocalTime().ToString("MM/dd/yyyy")
                    : "";
                ws.Cell(i + 2, 8).Value = r.Status ?? "";
                ws.Cell(i + 2, 9).Value = r.CurrentProcess ?? "";
                ws.Cell(i + 2, 10).Value = r.Remarks ?? "";

                var statusCell = ws.Cell(i + 2, 8);
                statusCell.Style.Font.Bold = true;
                statusCell.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                switch (r.Status)
                {
                    case "Finished":
                        statusCell.Style.Font.FontColor = ClosedXML.Excel.XLColor.FromHtml("#166534");
                        statusCell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#dcfce7");
                        break;
                    case "Delay":
                        statusCell.Style.Font.FontColor = ClosedXML.Excel.XLColor.FromHtml("#991b1b");
                        statusCell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#fee2e2");
                        break;
                    case "Ongoing":
                        statusCell.Style.Font.FontColor = ClosedXML.Excel.XLColor.FromHtml("#92400e");
                        statusCell.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#fef9c3");
                        break;
                }
            }

            ws.Columns().AdjustToContents();

            foreach (var col in ws.ColumnsUsed())
            {
                if (col.Width < 12) col.Width = 12;
                if (col.Width > 60) col.Width = 60;
            }

            ws.SheetView.FreezeRows(1);
            ws.RangeUsed().SetAutoFilter();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"AllPartsData_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            );
        }

        // ── Private helpers ────────────────────────────────────────

        private async Task<AllPartsDataViewModel> BuildViewModel()
        {
            var rows = await FetchRows(null, null);
            return new AllPartsDataViewModel
            {
                Parts = rows,
                FinishedCount = rows.Count(r => r.Status == "Finished"),
                OngoingCount = rows.Count(r => r.Status == "Ongoing"),
                DelayCount = rows.Count(r => r.Status == "Delay")
            };
        }

        private async Task<List<AllPartsDataRow>> FetchRows(string? activity, string? status)
        {
            // ── Load all tables needed ─────────────────────────────
            var importList = await _context.ImportDatas.ToListAsync();
            var leadTimes = await _context.LeadTimes.ToListAsync();
            var newToolingMappings = await _context.NewToolingProcessMappings.ToListAsync();
            var changeMaterialMappings = await _context.ChangeMaterialProcessMappings.ToListAsync();
            var other4MMappings = await _context.Other4MProcessMappings.ToListAsync();
            var today = DateTime.UtcNow;

            // Latest TransactionLogs entry PER (ControlNo, Activity) — single source of truth
            // for BOTH display (CurrentProcess shown on screen) AND status computation (Finished/Ongoing/Delay).
            var latestLogsPerActivity = await _context.TransactionLogs
                .GroupBy(x => new { x.TransactionNumber, x.Activity })
                .Select(g => g.OrderByDescending(x => x.InputDate).First())
                .ToListAsync();

            // Load remarks from each activity-specific process table
            var quotations = await _context.ToolingQuotationRequestApproval.ToListAsync();
            var requestOrders = await _context.MP2ToolingRequestOrder.ToListAsync();
            var poIssuances = await _context.MP2ToolingPoIssuance.ToListAsync();
            var dfmApprovals = await _context.SQCDFMQCDApprovals.ToListAsync();
            var fabrications = await _context.MP2ToolingFabrications.ToListAsync();
            var transfers = await _context.MP2ToolingTransfers.ToListAsync();
            var katakenSubs = await _context.IQCKatakenSubmissions.ToListAsync();
            var katakenFinish = await _context.IQCKatakenFinish.ToListAsync();
            var deEvals = await _context.DEEvaluation.ToListAsync();
            var qaEvals = await _context.QASpecialEvaluations.ToListAsync();
            var testRuns = await _context.IQCTestRuns.ToListAsync();

            // ── Completed sets — SAME source-of-truth tables as the Dashboard ──
            var completedRenewal = testRuns
                .Select(t => t.ControlNumber)
                .ToHashSet();

            var completedNewTooling = await _context.NewToolingLocalizationProcesses
                .Where(p => p.CurrentProcess == "Completed")
                .Select(p => p.ControlNumber)
                .ToListAsync();
            var completedNewToolingSet = completedNewTooling.ToHashSet();

            var completedChangeMaterial = await _context.ChangeMaterialProcesses
                .Where(p => p.ProcessStep == "First Delivery Date")
                .Select(p => p.ControlNumber)
                .ToListAsync();
            var completedChangeMaterialSet = completedChangeMaterial.ToHashSet();

            var completedOther4M = await _context.Other4MProcesses
                .Where(p => p.FirstDeliveryDate != null)
                .Select(p => p.ControlNumber)
                .ToListAsync();
            var completedOther4MSet = completedOther4M.ToHashSet();

            // ── Expand: one row per YES flag ───────────────────────
            var expandedRows = new List<(ImportData imp, string activity)>();

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
                        expandedRows.Add((imp, activityName));
                }
            }

            // ── Build rows ─────────────────────────────────────────
            var rows = expandedRows.Select(pair =>
            {
                var imp = pair.imp;
                var resolvedActivity = pair.activity;

                // ── Single source of truth for display: latest log scoped to THIS activity ──
                var latestLog = ActivityComputationHelper.GetLatestLogForActivity(
                    imp.ControlNo, resolvedActivity, latestLogsPerActivity);

                string displayCurrentProcess = latestLog?.CurrentProcess ?? "N/A";

                // ── Completed check — dedicated table per activity, SAME as Dashboard ──
                bool isCompleted = resolvedActivity switch
                {
                    "Renewal / Additional Mold" => completedRenewal.Contains(imp.ControlNo),
                    "New Tooling / Localization" => completedNewToolingSet.Contains(imp.ControlNo),
                    "Supplier Change / Localization" => completedNewToolingSet.Contains(imp.ControlNo),
                    "Multiple Procurement / Localization" => completedNewToolingSet.Contains(imp.ControlNo),
                    "Change Material" => completedChangeMaterialSet.Contains(imp.ControlNo),
                    "Other 4M" => completedOther4MSet.Contains(imp.ControlNo),
                    _ => false
                };

                // ── Status — identical helper call as Dashboard, guaranteed to match ──
                string resolvedStatus = ActivityComputationHelper.ResolveStatus(
                    isCompleted, latestLog, resolvedActivity,
                    leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today);

                // ── Remarks: pick from the latest process table ────
                var remarks = GetLatestRemarks(imp.ControlNo, displayCurrentProcess,
                    quotations, requestOrders, poIssuances, dfmApprovals,
                    fabrications, transfers, katakenSubs, katakenFinish,
                    deEvals, qaEvals, testRuns);

                return new AllPartsDataRow
                {
                    TransactionNumber = imp.ControlNo,
                    PartName = imp.PartName,
                    PartCode = imp.ChildPartcode,
                    Supplier = imp.Supplier,
                    Model = imp.Model,
                    Activity = resolvedActivity,
                    InputDate = imp.DateImported,
                    CurrentProcess = displayCurrentProcess,
                    Remarks = remarks,
                    Status = resolvedStatus
                };
            }).AsQueryable();

            // ── Filters ────────────────────────────────────────────
            if (!string.IsNullOrEmpty(activity) && activity != "All")
                rows = rows.Where(r => r.Activity == activity);

            if (!string.IsNullOrEmpty(status) && status != "All")
                rows = rows.Where(r => r.Status == status);

            return rows.OrderByDescending(r => r.InputDate).ToList();
        }

        // ── Returns remarks from whichever process table is most advanced ──
        private static string GetLatestRemarks(
            string controlNo,
            string currentProcess,
            List<MP2_ToolingQuotationRequestApproval> quotations,
            List<MP2_ToolingRequestOrder> requestOrders,
            List<MP2_ToolingPoIssuance> poIssuances,
            List<SQC_DFMQCDApproval> dfmApprovals,
            List<MP2_ToolingFabrication> fabrications,
            List<MP2_ToolingTransfer> transfers,
            List<IQC_KatakenSubmission> katakenSubs,
            List<IQC_KatakenFinish> katakenFinish,
            List<DE_Evaluation> deEvals,
            List<QA_SpecialEvaluation> qaEvals,
            List<IQC_TestRun> testRuns)
        {
            return
                GetRemarks(testRuns.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(qaEvals.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(deEvals.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(katakenFinish.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(katakenSubs.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(transfers.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(fabrications.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(dfmApprovals.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(poIssuances.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(requestOrders.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? GetRemarks(quotations.Where(r => r.ControlNumber == controlNo)
                    .OrderByDescending(r => r.CreateDate).FirstOrDefault(), r => r.Remarks)
                ?? "";
        }

        private static string? GetRemarks<T>(T? entity, Func<T, string?> selector) where T : class
        {
            if (entity == null) return null;
            var val = selector(entity);
            return string.IsNullOrWhiteSpace(val) ? null : val;
        }
    }
}