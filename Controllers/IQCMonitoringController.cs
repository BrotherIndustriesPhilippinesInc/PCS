using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
using PartsControlSystem.ViewModels;

namespace PartsControlSystem.Controllers
{
    [Route("IQCMonitoring")]
    public class IQCMonitoringController : Controller
    {
        private readonly PostgreAppDbContext _db;

        public IQCMonitoringController(PostgreAppDbContext db)
        {
            _db = db;
        }

        // =====================================================================
        // MAIN VIEW
        // =====================================================================
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var vm = await BuildViewModel();
            return View("IQCMonitoring", vm);
        }

        // =====================================================================
        // DOWNLOAD
        // =====================================================================
        [HttpGet("Download")]
        public async Task<IActionResult> Download(string activity)
        {
            var vm = await BuildViewModel();

            using var workbook = new XLWorkbook();

            switch (activity)
            {
                case "renewal":
                    BuildRenewalSheet(workbook, vm.RenewalRows);
                    break;
                case "multi":
                    BuildMultiSheet(workbook, vm.MultipleProcurementRows);
                    break;
                case "supplier":
                    BuildSupplierSheet(workbook, vm.SupplierChangeRows);
                    break;
                case "local":
                    BuildLocalizationSheet(workbook, vm.LocalizationRows);
                    break;
                case "change":
                    BuildChangeMaterialSheet(workbook, vm.ChangeMaterialRows);
                    break;
                default:
                    BuildRenewalSheet(workbook, vm.RenewalRows);
                    break;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"IQC_Monitoring_{activity}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
            );
        }

        // =====================================================================
        // BUILD VIEW MODEL
        // =====================================================================
        private async Task<IQCMonitoringViewModel> BuildViewModel()
        {
            var vm = new IQCMonitoringViewModel();

            // ── shared lookups ──────────────────────────────────────────────
            var importDatas = await _db.ImportDatas.AsNoTracking().ToListAsync();

            var latestProcesses = await _db.ActivityCurrentProcesses
                .AsNoTracking()
                .GroupBy(x => x.ControlNumber)
                .Select(g => g.OrderByDescending(x => x.UpdateAt).First())
                .ToListAsync();

            string GetPending(string controlNo)
                => latestProcesses.FirstOrDefault(p => p.ControlNumber == controlNo)?.CurrentProcess ?? "—";

            // ── RENEWAL / ADDITIONAL MOLD ───────────────────────────────────
            var renewalImports = importDatas.Where(x => x.RenewalAdditionalMold == "YES").ToList();

            var katakenFinishList = await _db.IQCKatakenFinish.AsNoTracking().ToListAsync();
            var katakenSubList = await _db.IQCKatakenSubmissions.AsNoTracking().ToListAsync();
            var testRunList = await _db.IQCTestRuns.AsNoTracking().ToListAsync();

            foreach (var imp in renewalImports)
            {
                var kf = katakenFinishList.FirstOrDefault(x => x.ControlNumber == imp.ControlNo);
                var ks = katakenSubList.FirstOrDefault(x => x.ControlNumber == imp.ControlNo);
                var tr = testRunList.FirstOrDefault(x => x.ControlNumber == imp.ControlNo);

                vm.RenewalRows.Add(new IQCRenewalMonitoringRow
                {
                    TransactionNumber = imp.ControlNo,
                    ToolingType = imp.ToolingType,
                    Category = imp.ToolingCategory,
                    Model = imp.Model,
                    PartCode = imp.ChildPartcode,
                    PendingItems = GetPending(imp.ControlNo),

                    KatakenFinishTarget = kf?.ActualFinishDate,
                    KatakenFinishActual = kf?.ActualFinishDate,
                    KatakenFinishStatus = kf != null ? kf.Result : null,
                    KatakenFinishRemarks = kf?.Remarks,

                    KatakenSubTarget = ks?.ActualSubmissionDate,
                    KatakenSubActual = ks?.ActualSubmissionDate,
                    KatakenSubStatus = ks != null ? "Done" : null,
                    KatakenSubRemarks = ks?.Remarks,

                    TestRunTarget = tr?.ActualFinishDate,
                    TestRunActual = tr?.ActualFinishDate,
                    TestRunStatus = tr?.ResultPassedFailed,
                    TestRunRemarks = tr?.Remarks,
                });
            }

            // ── MULTIPLE PROCUREMENT ────────────────────────────────────────
            var mpImports = importDatas.Where(x => x.MultipleProcurementLocalization == "YES").ToList();
            var mpView = await _db.ViewMultipleProcurementMonitoring.AsNoTracking().ToListAsync();

            foreach (var imp in mpImports)
            {
                var row = mpView.FirstOrDefault(x => x.ControlNumber == imp.ControlNo);

                vm.MultipleProcurementRows.Add(new IQCMultipleProcurementMonitoringRow
                {
                    TransactionNumber = imp.ControlNo,
                    ToolingType = imp.ToolingType,
                    Category = imp.ToolingCategory,
                    Model = imp.Model,
                    PartCode = imp.ChildPartcode,
                    PendingItems = GetPending(imp.ControlNo),

                    KatakenSubTarget = row?.kataken_sub_target,
                    KatakenSubActual = row?.kataken_sub_actual,
                    KatakenSubStatus = row?.kataken_sub_status,
                    KatakenSubRemarks = null,

                    KatakenApprTarget = row?.kataken_appr_target,
                    KatakenApprActual = row?.kataken_appr_actual,
                    KatakenApprStatus = row?.kataken_appr_status,
                    KatakenApprRemarks = null,

                    PPSTarget = row?.pps_target,
                    PPSActual = row?.pps_approval_date,
                    PPSStatus = row?.pps_available,
                    PPSRemarks = null,

                    TestRunPOReqTarget = row?.test_run_po_req_target,
                    TestRunPOReqActual = row?.test_run_po_req_actual,
                    TestRunPOReqStatus = null,
                    TestRunPOReqRemarks = null,

                    TestRunSchedStart = row?.test_run_sched_start,
                    TestRunSchedFinish = row?.test_run_sched_finish,
                    TestRunSchedStatus = null,
                    TestRunSchedRemarks = null,

                    TestRunApprTarget = row?.test_run_appr_target,
                    TestRunApprActual = row?.test_run_appr_actual,
                    TestRunApprStatus = row?.test_run_appr_status,
                    TestRunApprRemarks = null,
                });
            }

            // ── SUPPLIER CHANGE ─────────────────────────────────────────────
            var scImports = importDatas.Where(x => x.SupplierChangeLocalization == "YES").ToList();
            var scView = await _db.ViewSupplierChangeMonitoring.AsNoTracking().ToListAsync();

            foreach (var imp in scImports)
            {
                var row = scView.FirstOrDefault(x => x.ControlNumber == imp.ControlNo);

                vm.SupplierChangeRows.Add(new IQCSupplierChangeMonitoringRow
                {
                    TransactionNumber = imp.ControlNo,
                    ToolingType = imp.ToolingType,
                    Category = imp.ToolingCategory,
                    Model = imp.Model,
                    PartCode = imp.ChildPartcode,
                    PendingItems = GetPending(imp.ControlNo),

                    KatakenSubTarget = row?.kataken_sub_target,
                    KatakenSubActual = row?.kataken_sub_actual,
                    KatakenSubStatus = null,
                    KatakenSubRemarks = null,

                    KatakenApprTarget = row?.kataken_appr_target,
                    KatakenApprActual = row?.kataken_appr_actual,
                    KatakenApprStatus = null,
                    KatakenApprRemarks = null,

                    PPSTarget = row?.pps_target,
                    PPSActual = row?.pps_approval_date,
                    PPSStatus = null,
                    PPSRemarks = null,

                    TestRunPOReqTarget = row?.test_run_po_req_target,
                    TestRunPOReqActual = null,
                    TestRunPOReqStatus = null,
                    TestRunPOReqRemarks = null,

                    TestRunSchedStart = row?.test_run_sched_start,
                    TestRunSchedFinish = null,
                    TestRunSchedStatus = null,
                    TestRunSchedRemarks = null,

                    TestRunApprTarget = row?.test_run_appr_target,
                    TestRunApprActual = row?.test_run_appr_actual,
                    TestRunApprStatus = null,
                    TestRunApprRemarks = null,

                    FourMApprTarget = row?.approval_date_4m_target,
                    FourMApprActual = null,
                    FourMApprStatus = null,
                    FourMApprRemarks = null,
                });
            }

            // ── LOCALIZATION ────────────────────────────────────────────────
            var locImports = importDatas.Where(x => x.NewToolingLocalization == "YES").ToList();
            var locView = await _db.ViewLocalizationMonitoring.AsNoTracking().ToListAsync();

            foreach (var imp in locImports)
            {
                var row = locView.FirstOrDefault(x => x.ControlNumber == imp.ControlNo);

                vm.LocalizationRows.Add(new IQCLocalizationMonitoringRow
                {
                    TransactionNumber = imp.ControlNo,
                    ToolingType = imp.ToolingType,
                    Category = imp.ToolingCategory,
                    Model = imp.Model,
                    PartCode = imp.ChildPartcode,
                    PendingItems = GetPending(imp.ControlNo),

                    KatakenSubTarget = row?.kataken_sub_target,
                    KatakenSubActual = row?.kataken_sub_actual,
                    KatakenSubStatus = null,
                    KatakenSubRemarks = null,

                    KatakenApprTarget = row?.kataken_appr_target,
                    KatakenApprActual = row?.kataken_appr_actual,
                    KatakenApprStatus = null,
                    KatakenApprRemarks = null,

                    TestRunPOReqTarget = row?.test_run_po_req_target,
                    TestRunPOReqActual = null,
                    TestRunPOReqStatus = null,
                    TestRunPOReqRemarks = null,

                    TestRunSchedStart = row?.test_run_sched_start,
                    TestRunSchedFinish = null,
                    TestRunSchedStatus = null,
                    TestRunSchedRemarks = null,

                    TestRunApprTarget = row?.test_run_appr_target,
                    TestRunApprActual = row?.test_run_appr_actual,
                    TestRunApprStatus = null,
                    TestRunApprRemarks = null,

                    FourMApprTarget = row?.approval_date_4m_target,
                    FourMApprActual = null,
                    FourMApprStatus = null,
                    FourMApprRemarks = null,
                });
            }

            // ── CHANGE MATERIAL ─────────────────────────────────────────────
            var cmImports = importDatas.Where(x => x.ChangeMaterial == "YES").ToList();
            var cmView = await _db.ViewChangeMaterialMonitoring.AsNoTracking().ToListAsync();

            foreach (var imp in cmImports)
            {
                var row = cmView.FirstOrDefault(x => x.ControlNumber == imp.ControlNo);

                vm.ChangeMaterialRows.Add(new IQCChangeMaterialMonitoringRow
                {
                    TransactionNumber = imp.ControlNo,
                    ToolingType = imp.ToolingType,
                    Category = imp.ToolingCategory,
                    Model = imp.Model,
                    PartCode = imp.ChildPartcode,
                    PendingItems = GetPending(imp.ControlNo),

                    KatakenSubTarget = row?.KatakenPhTargetDate,
                    KatakenSubActual = row?.KatakenPhActualDate,
                    KatakenSubStatus = row?.KatakenPhActualDate.HasValue == true ? "Done" : null,
                    KatakenSubRemarks = null,

                    KatakenEvalTarget = row?.KatakenEvalTargetDate,
                    KatakenEvalActual = row?.KatakenEvalActualDate,
                    KatakenEvalStatus = row?.KatakenEvalActualDate.HasValue == true ? "Done" : null,
                    KatakenEvalRemarks = null,

                    QAEvalTarget = row?.QaEvalTargetDate,
                    QAEvalActual = row?.QaEvalActualDate,
                    QAEvalStatus = row?.QaEvalActualDate.HasValue == true ? "Done" : null,
                    QAEvalRemarks = null,

                    DEEvalTarget = row?.DeEvalTargetDate,
                    DEEvalActual = row?.DeEvalActualDate,
                    DEEvalStatus = row?.DeEvalActualDate.HasValue == true ? "Done" : null,
                    DEEvalRemarks = null,

                    TestRunTarget = row?.TestRunTargetDate,
                    TestRunActual = row?.TestRunActualDate,
                    TestRunStatus = row?.TestRunActualDate.HasValue == true ? "Done" : null,
                    TestRunRemarks = null,
                });
            }

            return vm;
        }

        // =====================================================================
        // CLOSEDXML SHEET BUILDERS
        // =====================================================================

        private static string Fmt(DateTime? d) =>
            d.HasValue ? d.Value.ToLocalTime().ToString("MM/dd/yyyy") : "";

        private static void ApplyHeaderStyle(IXLCell cell, string hexColor, bool darkText = false)
        {
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#" + hexColor);
            cell.Style.Font.FontColor = darkText ? XLColor.Black : XLColor.White;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 10;
            cell.Style.Font.FontName = "Arial";
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = XLColor.Black;
        }

        private static void ApplyDataStyle(IXLCell cell, bool center = false, bool pinkBg = false)
        {
            cell.Style.Font.FontSize = 10;
            cell.Style.Font.FontName = "Arial";
            cell.Style.Alignment.Horizontal = center
                ? XLAlignmentHorizontalValues.Center
                : XLAlignmentHorizontalValues.Left;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = XLColor.Black;
            if (pinkBg)
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#ffcdff");
        }

        // ── Writes Transaction Number, Information band, Pending Items (rows 1-3, cols 1-6) ──
        private static void WriteInfoHeaders(IXLWorksheet ws)
        {
            // Transaction Number — merged rows 1-3
            var txn = ws.Cell(1, 1);
            txn.Value = "Transaction Number";
            ApplyHeaderStyle(txn, "1f3864");
            ws.Range(1, 1, 3, 1).Merge();

            // Information band — merged row 1, cols 2-5
            var info = ws.Cell(1, 2);
            info.Value = "Information";
            ApplyHeaderStyle(info, "00b0f0");
            ws.Range(1, 2, 1, 5).Merge();

            // Pending Items — merged rows 1-3
            var pend = ws.Cell(1, 6);
            pend.Value = "Pending Items";
            ApplyHeaderStyle(pend, "ff00ff");
            ws.Range(1, 6, 3, 6).Merge();

            // Info sub-headers — merged rows 2-3 each
            var infoSubs = new[] { "Tooling Type", "Category", "Model", "Part Code" };
            for (int i = 0; i < infoSubs.Length; i++)
            {
                var c = ws.Cell(2, 2 + i);
                c.Value = infoSubs[i];
                ApplyHeaderStyle(c, "00b0f0");
                ws.Range(2, 2 + i, 3, 2 + i).Merge();
            }
        }

        // ── Writes activity band + group headers + Target/Actual/Remarks sub-headers ──
        private static void WriteActivityHeaders(
            IXLWorksheet ws,
            string activityLabel,
            string bandHex,
            string groupHex,
            string subHdrHex,
            bool darkText,
            string[] groupNames,
            int startCol)
        {
            int totalCols = groupNames.Length * 3;

            // Row 1 — Activity band
            var band = ws.Cell(1, startCol);
            band.Value = activityLabel;
            ApplyHeaderStyle(band, bandHex, darkText);
            ws.Range(1, startCol, 1, startCol + totalCols - 1).Merge();

            // Row 2 — Group headers
            int col = startCol;
            foreach (var grp in groupNames)
            {
                var g = ws.Cell(2, col);
                g.Value = grp;
                ApplyHeaderStyle(g, groupHex, darkText);
                ws.Range(2, col, 2, col + 2).Merge();
                col += 3;
            }

            // Row 3 — Target / Actual / Remarks
            col = startCol;
            foreach (var _ in groupNames)
            {
                var t = ws.Cell(3, col); t.Value = "Target"; ApplyHeaderStyle(t, subHdrHex, true);
                var a = ws.Cell(3, col + 1); a.Value = "Actual"; ApplyHeaderStyle(a, subHdrHex, true);
                var r = ws.Cell(3, col + 2); r.Value = "Remarks"; ApplyHeaderStyle(r, subHdrHex, true);
                col += 3;
            }
        }

        // ── Sets column widths and row heights ──
        private static void SetColumnWidths(IXLWorksheet ws, int groupCount)
        {
            ws.Column(1).Width = 22; // Transaction Number
            ws.Column(2).Width = 14; // Tooling Type
            ws.Column(3).Width = 12; // Category
            ws.Column(4).Width = 12; // Model
            ws.Column(5).Width = 14; // Part Code
            ws.Column(6).Width = 30; // Pending Items
            for (int i = 0; i < groupCount; i++)
            {
                ws.Column(7 + i * 3).Width = 13; // Target
                ws.Column(7 + i * 3 + 1).Width = 13; // Actual
                ws.Column(7 + i * 3 + 2).Width = 22; // Remarks
            }
            ws.Row(1).Height = 32;
            ws.Row(2).Height = 32;
            ws.Row(3).Height = 20;
        }

        // ── Writes a single data row ──
        private static void WriteDataRow(
            IXLWorksheet ws,
            int rowNum,
            string txn,
            string toolingType,
            string category,
            string model,
            string partCode,
            string pendingItems,
            params string[] dataCells)
        {
            bool isAlt = rowNum % 2 == 0;
            var altColor = XLColor.FromHtml("#f0f4f8");

            void Write(int col, string val, bool center = false, bool pink = false)
            {
                var cell = ws.Cell(rowNum, col);
                cell.Value = val ?? "";
                ApplyDataStyle(cell, center, pink);
                if (!pink && isAlt)
                    cell.Style.Fill.BackgroundColor = altColor;
            }

            Write(1, txn);
            Write(2, toolingType);
            Write(3, category);
            Write(4, model);
            Write(5, partCode);
            Write(6, pendingItems, false, true); // pink background

            for (int i = 0; i < dataCells.Length; i++)
            {
                bool isCenter = i % 3 != 2; // Target + Actual centered, Remarks left
                Write(7 + i, dataCells[i], isCenter);
            }

            ws.Row(rowNum).Height = 18;
        }

        // ── RENEWAL ──────────────────────────────────────────────────────────
        private static void BuildRenewalSheet(XLWorkbook wb, List<IQCRenewalMonitoringRow> rows)
        {
            var ws = wb.Worksheets.Add("Renewal");
            WriteInfoHeaders(ws);
            WriteActivityHeaders(ws,
                "Renewal / Additional Mold",
                bandHex: "92d050", groupHex: "70ad47", subHdrHex: "d4edda",
                darkText: false,
                groupNames: new[] {
                    "Kataken Finish (Local Trial)",
                    "Kataken Submission (Local Trial)",
                    "Test Run"
                },
                startCol: 7);
            SetColumnWidths(ws, 3);

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                WriteDataRow(ws, 4 + i,
                    r.TransactionNumber, r.ToolingType, r.Category, r.Model, r.PartCode,
                    r.PendingItems ?? "",
                    Fmt(r.KatakenFinishTarget), Fmt(r.KatakenFinishActual), r.KatakenFinishRemarks ?? "",
                    Fmt(r.KatakenSubTarget), Fmt(r.KatakenSubActual), r.KatakenSubRemarks ?? "",
                    Fmt(r.TestRunTarget), Fmt(r.TestRunActual), r.TestRunRemarks ?? "");
            }

            ws.SheetView.FreezeRows(3);
            if (ws.RangeUsed() != null) ws.RangeUsed().SetAutoFilter();
        }

        // ── MULTIPLE PROCUREMENT ──────────────────────────────────────────────
        private static void BuildMultiSheet(XLWorkbook wb, List<IQCMultipleProcurementMonitoringRow> rows)
        {
            var ws = wb.Worksheets.Add("Multiple Procurement");
            WriteInfoHeaders(ws);
            WriteActivityHeaders(ws,
                "Multiple Procurement / Localization",
                bandHex: "ff9999", groupHex: "ff9999", subHdrHex: "ffb6c1",
                darkText: false,
                groupNames: new[] {
                    "Kataken PH Sample Submission",
                    "Kataken PH Sample Approval",
                    "Availability of Parts Packaging Standard",
                    "Test Run PO Request Date",
                    "Test Run Schedule",
                    "Test Run Approval Date"
                },
                startCol: 7);
            SetColumnWidths(ws, 6);

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                WriteDataRow(ws, 4 + i,
                    r.TransactionNumber, r.ToolingType, r.Category, r.Model, r.PartCode,
                    r.PendingItems ?? "",
                    Fmt(r.KatakenSubTarget), Fmt(r.KatakenSubActual), r.KatakenSubRemarks ?? "",
                    Fmt(r.KatakenApprTarget), Fmt(r.KatakenApprActual), r.KatakenApprRemarks ?? "",
                    Fmt(r.PPSTarget), Fmt(r.PPSActual), r.PPSRemarks ?? "",
                    Fmt(r.TestRunPOReqTarget), Fmt(r.TestRunPOReqActual), r.TestRunPOReqRemarks ?? "",
                    Fmt(r.TestRunSchedStart), Fmt(r.TestRunSchedFinish), r.TestRunSchedRemarks ?? "",
                    Fmt(r.TestRunApprTarget), Fmt(r.TestRunApprActual), r.TestRunApprRemarks ?? "");
            }

            ws.SheetView.FreezeRows(3);
            if (ws.RangeUsed() != null) ws.RangeUsed().SetAutoFilter();
        }

        // ── SUPPLIER CHANGE ───────────────────────────────────────────────────
        private static void BuildSupplierSheet(XLWorkbook wb, List<IQCSupplierChangeMonitoringRow> rows)
        {
            var ws = wb.Worksheets.Add("Supplier Change");
            WriteInfoHeaders(ws);
            WriteActivityHeaders(ws,
                "Supplier Change / Localization",
                bandHex: "ff0000", groupHex: "c00000", subHdrHex: "ff4d4d",
                darkText: false,
                groupNames: new[] {
                    "Kataken PH Sample Submission",
                    "Kataken PH Sample Approval",
                    "Availability of Parts Packaging Standard",
                    "Test Run PO Request Date",
                    "Test Run Schedule",
                    "Test Run Approval Date",
                    "4M Approval Date"
                },
                startCol: 7);
            SetColumnWidths(ws, 7);

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                WriteDataRow(ws, 4 + i,
                    r.TransactionNumber, r.ToolingType, r.Category, r.Model, r.PartCode,
                    r.PendingItems ?? "",
                    Fmt(r.KatakenSubTarget), Fmt(r.KatakenSubActual), r.KatakenSubRemarks ?? "",
                    Fmt(r.KatakenApprTarget), Fmt(r.KatakenApprActual), r.KatakenApprRemarks ?? "",
                    Fmt(r.PPSTarget), Fmt(r.PPSActual), r.PPSRemarks ?? "",
                    Fmt(r.TestRunPOReqTarget), Fmt(r.TestRunPOReqActual), r.TestRunPOReqRemarks ?? "",
                    Fmt(r.TestRunSchedStart), Fmt(r.TestRunSchedFinish), r.TestRunSchedRemarks ?? "",
                    Fmt(r.TestRunApprTarget), Fmt(r.TestRunApprActual), r.TestRunApprRemarks ?? "",
                    Fmt(r.FourMApprTarget), Fmt(r.FourMApprActual), r.FourMApprRemarks ?? "");
            }

            ws.SheetView.FreezeRows(3);
            if (ws.RangeUsed() != null) ws.RangeUsed().SetAutoFilter();
        }

        // ── LOCALIZATION ──────────────────────────────────────────────────────
        private static void BuildLocalizationSheet(XLWorkbook wb, List<IQCLocalizationMonitoringRow> rows)
        {
            var ws = wb.Worksheets.Add("New Tooling Localization");
            WriteInfoHeaders(ws);
            WriteActivityHeaders(ws,
                "New Tooling / Localization",
                bandHex: "7030a0", groupHex: "9b59d0", subHdrHex: "c598f1",
                darkText: false,
                groupNames: new[] {
                    "Kataken PH Sample Submission",
                    "Kataken PH Sample Approval",
                    "Test Run PO Request Date",
                    "Test Run Schedule",
                    "Test Run Approval Date",
                    "4M Approval Date"
                },
                startCol: 7);
            SetColumnWidths(ws, 6);

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                WriteDataRow(ws, 4 + i,
                    r.TransactionNumber, r.ToolingType, r.Category, r.Model, r.PartCode,
                    r.PendingItems ?? "",
                    Fmt(r.KatakenSubTarget), Fmt(r.KatakenSubActual), r.KatakenSubRemarks ?? "",
                    Fmt(r.KatakenApprTarget), Fmt(r.KatakenApprActual), r.KatakenApprRemarks ?? "",
                    Fmt(r.TestRunPOReqTarget), Fmt(r.TestRunPOReqActual), r.TestRunPOReqRemarks ?? "",
                    Fmt(r.TestRunSchedStart), Fmt(r.TestRunSchedFinish), r.TestRunSchedRemarks ?? "",
                    Fmt(r.TestRunApprTarget), Fmt(r.TestRunApprActual), r.TestRunApprRemarks ?? "",
                    Fmt(r.FourMApprTarget), Fmt(r.FourMApprActual), r.FourMApprRemarks ?? "");
            }

            ws.SheetView.FreezeRows(3);
            if (ws.RangeUsed() != null) ws.RangeUsed().SetAutoFilter();
        }

        // ── CHANGE MATERIAL ───────────────────────────────────────────────────
        private static void BuildChangeMaterialSheet(XLWorkbook wb, List<IQCChangeMaterialMonitoringRow> rows)
        {
            var ws = wb.Worksheets.Add("Change Material");
            WriteInfoHeaders(ws);
            WriteActivityHeaders(ws,
                "Change Material",
                bandHex: "e6e600", groupHex: "ffff00", subHdrHex: "ffff81",
                darkText: true,
                groupNames: new[] {
                    "Kataken PH Sample Submission",
                    "Kataken Evaluation Approval",
                    "QA Evaluation",
                    "DE Evaluation",
                    "Test Run"
                },
                startCol: 7);
            SetColumnWidths(ws, 5);

            for (int i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                WriteDataRow(ws, 4 + i,
                    r.TransactionNumber, r.ToolingType, r.Category, r.Model, r.PartCode,
                    r.PendingItems ?? "",
                    Fmt(r.KatakenSubTarget), Fmt(r.KatakenSubActual), r.KatakenSubRemarks ?? "",
                    Fmt(r.KatakenEvalTarget), Fmt(r.KatakenEvalActual), r.KatakenEvalRemarks ?? "",
                    Fmt(r.QAEvalTarget), Fmt(r.QAEvalActual), r.QAEvalRemarks ?? "",
                    Fmt(r.DEEvalTarget), Fmt(r.DEEvalActual), r.DEEvalRemarks ?? "",
                    Fmt(r.TestRunTarget), Fmt(r.TestRunActual), r.TestRunRemarks ?? "");
            }

            ws.SheetView.FreezeRows(3);
            if (ws.RangeUsed() != null) ws.RangeUsed().SetAutoFilter();
        }
    }
}