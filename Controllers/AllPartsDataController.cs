using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
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

            // Apply month/year filter to match what's visible on screen
            if (!string.IsNullOrEmpty(month) && month != "All" && int.TryParse(month, out int m))
                rows = rows.Where(r => r.InputDate.HasValue && r.InputDate.Value.ToLocalTime().Month == m).ToList();

            if (!string.IsNullOrEmpty(year) && year != "All" && int.TryParse(year, out int y))
                rows = rows.Where(r => r.InputDate.HasValue && r.InputDate.Value.ToLocalTime().Year == y).ToList();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.Worksheets.Add("All Parts Data");

            // ── Header row ─────────────────────────────────────────────
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

            // ── Data rows ───────────────────────────────────────────────
            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                var row = ws.Row(i + 2);

                // Alternating row color
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

                // Status cell color
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

            // ── Auto-fit all columns ────────────────────────────────────
            ws.Columns().AdjustToContents();

            // ── Set minimum column widths ───────────────────────────────
            foreach (var col in ws.ColumnsUsed())
            {
                if (col.Width < 12) col.Width = 12;
                if (col.Width > 60) col.Width = 60;
            }

            // ── Freeze header row ───────────────────────────────────────
            ws.SheetView.FreezeRows(1);

            // ── Add auto-filter ─────────────────────────────────────────
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
            var procList = await _context.ActivityCurrentProcesses.ToListAsync();
            var leadTimes = await _context.LeadTimes.ToListAsync();
            var today = DateTime.UtcNow;

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

                // Latest process entry for this control number
                var latestProc = procList
                    .Where(p => p.ControlNumber == imp.ControlNo)
                    .OrderByDescending(p => p.UpdateAt)
                    .FirstOrDefault();

                var currentProcess = latestProc?.CurrentProcess ?? "N/A";

                // ── Status logic — mirrors HomeController exactly ──
                string resolvedStatus;

                if (latestProc == null)
                {
                    resolvedStatus = "Ongoing";
                }
                else if (IsCompleted(resolvedActivity, currentProcess))
                {
                    resolvedStatus = "Finished";
                }
                else
                {
                    // Check deadline: UpdateAt + LeadTimeValue < today → Delay
                    var leadTime = leadTimes
                        .FirstOrDefault(lt => lt.Activity == currentProcess);

                    if (leadTime != null)
                    {
                        var deadline = latestProc.UpdateAt.AddDays((double)leadTime.LeadTimeValue);
                        resolvedStatus = deadline < today ? "Delay" : "Ongoing";
                    }
                    else
                    {
                        resolvedStatus = "Ongoing";
                    }
                }

                // ── Remarks: pick from the latest process table ────
                var remarks = GetLatestRemarks(imp.ControlNo, currentProcess,
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
                    CurrentProcess = currentProcess,
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

        // ── Mirrors IsCompleted from TransactionLogsController ─────
        private static bool IsCompleted(string? activity, string currentProcess)
        {
            if (string.IsNullOrWhiteSpace(currentProcess)) return false;

            if (activity == "Renewal / Additional Mold")
                return currentProcess == "MP2-PDC";

            if (activity == "Change Material")
                return currentProcess == "First Delivery Date";

            return currentProcess == "Completed";
        }
    }
}