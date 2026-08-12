using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.DTO;
using PartsControlSystem.Helpers;
using PartsControlSystem.Models;
using PartsControlSystem.Services;
using PartsControlSystem.ViewModels;
//using static Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAlrunsRecord;

namespace PartsControlSystem.Controllers
{
    [Route("UpdatePerActivity")]
    public class UpdatePerActivityController : Controller
    {
        private readonly PostgreAppDbContext _dbContext;
        private readonly IUpdateActivityMapperService _updateActivityMapperService;

        public UpdatePerActivityController(PostgreAppDbContext dbContext, IUpdateActivityMapperService updateActivityMapperService)
        {
            _dbContext = dbContext;
            _updateActivityMapperService = updateActivityMapperService;
        }

        // =====================================================================
        // DASHBOARD / MONITORING
        // =====================================================================

        [HttpGet("ActivityMonitoring")]
        public async Task<IActionResult> ActivityMonitoring()
        {
            var section = User.FindFirst("Section")?.Value;
            var allowedActivities = GetAllowedActivitiesForSection(section);

            ViewBag.AllowedActivities = allowedActivities;
            ViewBag.CurrentSection = section;

            // Load default activity's data — first allowed activity, or empty if none
            if (!allowedActivities.Contains("Renewal / Additional Mold"))
            {
                // Section has no access to Renewal — don't preload it, view will handle empty state
                ViewBag.DefaultActivity = allowedActivities.FirstOrDefault();
                var emptyData = new List<ViewActivityMonitoring>();
                return View(emptyData);
            }

            var data = await _dbContext.ViewActivityMonitoring
                .AsNoTracking()
                .OrderBy(x => x.ControlNumber)
                .ToListAsync();

            ViewBag.DefaultActivity = "Renewal / Additional Mold";
            return View(data);
        }

        [HttpGet("GetMonitoringPartial")]
        public async Task<IActionResult> GetMonitoringPartial(string activity)
        {
            var section = User.FindFirst("Section")?.Value;
            var allowedActivities = GetAllowedActivitiesForSection(section);

            if (!allowedActivities.Contains(activity))
            {
                return Content(@"
        <div class='alert alert-danger text-center mt-4'>
            <i class='fa-solid fa-lock me-2'></i>
            Your section does not have access to this activity's monitoring data.
        </div>");
            }

            switch (activity)
            {
                case "Renewal / Additional Mold":
                    {
                        var data = await _dbContext.ViewActivityMonitoring
                            .AsNoTracking()
                            .OrderBy(x => x.ControlNumber)
                            .ToListAsync();
                        return PartialView("Partials/Monitoring/_RenewalMonitoring", data);
                    }
                case "New Tooling / Localization":
                    {
                        var data = await _dbContext.ViewLocalizationMonitoring
                            .AsNoTracking()
                            .OrderBy(x => x.ControlNumber)
                            .ToListAsync();
                        return PartialView("Partials/Monitoring/_LocalizationMonitoring", data);
                    }
                case "Supplier Change / Localization":
                    {
                        var data = await _dbContext.ViewSupplierChangeMonitoring
                            .AsNoTracking()
                            .OrderBy(x => x.ControlNumber)
                            .ToListAsync();
                        return PartialView("Partials/Monitoring/_SupplierChangeMonitoring", data);
                    }
                case "Multiple Procurement / Localization":
                    {
                        var data = await _dbContext.ViewMultipleProcurementMonitoring
                            .AsNoTracking()
                            .OrderBy(x => x.ControlNumber)
                            .ToListAsync();
                        return PartialView("Partials/Monitoring/_MultipleProcurementMonitoring", data);
                    }
                case "Change Material":
                    {
                        var data = await _dbContext.ViewChangeMaterialMonitoring
                            .AsNoTracking()
                            .OrderBy(x => x.ControlNumber)
                            .ToListAsync();
                        return PartialView("Partials/Monitoring/_ChangeMaterialMonitoring", data);
                    }
                case "Other 4M":
                    {
                        var data = await _dbContext.ViewOther4MMonitoring
                            .AsNoTracking()
                            .OrderBy(x => x.ControlNumber)
                            .ToListAsync();
                        return PartialView("Partials/Monitoring/_Other4MMonitoring", data);
                    }
                default:
                    return Content(@"
        <div class='alert alert-info text-center mt-4'>
            <i class='fa-solid fa-circle-info me-2'></i>
            No monitoring data available for this activity yet.
        </div>");
            }
        }

        // =====================================================================
        // UPDATE ACTIVITY
        // =====================================================================

        [HttpGet("UpdateActivity")]
        public async Task<IActionResult> UpdateActivity()
        {
            var data = await _dbContext.ImportDatas.ToListAsync();
            int totalEntries = data.Count;

            var allSections = data
                .Select(x => x.Section ?? "N/A")
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            List<SectionSummaryViewModel> GetSectionCounts(Func<ImportData, string> selector)
            {
                return allSections
                    .Select(section => new SectionSummaryViewModel
                    {
                        Section = section,
                        Count = data
                            .Where(x => (x.Section ?? "N/A") == section &&
                                        !string.IsNullOrWhiteSpace(selector(x)) &&
                                        selector(x).Trim().Equals("YES", StringComparison.OrdinalIgnoreCase))
                            .Count()
                    })
                    .ToList();
            }

            var today = DateTime.UtcNow;
            var leadTimes = await _dbContext.LeadTimes.ToListAsync();
            var newToolingMappings = await _dbContext.NewToolingProcessMappings.ToListAsync();
            var changeMaterialMappings = await _dbContext.ChangeMaterialProcessMappings.ToListAsync();
            var other4MMappings = await _dbContext.Other4MProcessMappings.ToListAsync();

            // Single source of truth — same as Dashboard/AllPartsData/TransactionLogs
            var latestLogsPerActivity = await _dbContext.TransactionLogs
                .GroupBy(x => new { x.TransactionNumber, x.Activity })
                .Select(g => g.OrderByDescending(x => x.InputDate).First())
                .ToListAsync();

            int GetDelayCount(string activityName, Func<ImportData, string> activitySelector, Func<string, bool> isCompleted)
            {
                var activityRecords = data
                    .Where(x => activitySelector(x)
                        .Trim().Equals("YES", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                int delayCount = 0;

                foreach (var record in activityRecords)
                {
                    if (isCompleted(record.ControlNo)) continue;

                    var latestLog = ActivityComputationHelper.GetLatestLogForActivity(
                        record.ControlNo, activityName, latestLogsPerActivity);

                    var status = ActivityComputationHelper.ResolveStatus(
                        isCompleted: false,
                        latestLog,
                        activityName,
                        leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today);

                    if (status == "Delay") delayCount++;
                }

                return delayCount;
            }

            var activityCards = new List<ActivityCardViewModel>
            {
                new ActivityCardViewModel
                {
                    ActivityName  = "Renewal / Additional Mold",
                    SectionCounts = GetSectionCounts(x => x.RenewalAdditionalMold),
                    TotalDelay    = GetDelayCount(
                        "Renewal / Additional Mold",
                        x => x.RenewalAdditionalMold,
                        controlNo => _dbContext.IQCTestRuns.Any(x => x.ControlNumber == controlNo))
                },
                new ActivityCardViewModel
                {
                    ActivityName  = "New Tooling / Localization",
                    SectionCounts = GetSectionCounts(x => x.NewToolingLocalization),
                    TotalDelay    = GetDelayCount(
                        "New Tooling / Localization",
                        x => x.NewToolingLocalization,
                        controlNo => _dbContext.NewToolingLocalizationProcesses
                            .Any(x => x.ControlNumber == controlNo && x.CurrentProcess == "Completed"))
                },
                new ActivityCardViewModel
                {
                    ActivityName  = "Supplier Change / Localization",
                    SectionCounts = GetSectionCounts(x => x.SupplierChangeLocalization),
                    TotalDelay    = GetDelayCount(
                        "Supplier Change / Localization",
                        x => x.SupplierChangeLocalization,
                        controlNo => _dbContext.NewToolingLocalizationProcesses
                            .Any(x => x.ControlNumber == controlNo && x.CurrentProcess == "Completed"))
                },
                new ActivityCardViewModel
                {
                    ActivityName  = "Multiple Procurement / Localization",
                    SectionCounts = GetSectionCounts(x => x.MultipleProcurementLocalization),
                    TotalDelay    = GetDelayCount(
                        "Multiple Procurement / Localization",
                        x => x.MultipleProcurementLocalization,
                        controlNo => _dbContext.NewToolingLocalizationProcesses
                            .Any(x => x.ControlNumber == controlNo && x.CurrentProcess == "Completed"))
                },
                //new ActivityCardViewModel { ActivityName = "Transfer Tooling", SectionCounts = GetSectionCounts(x => x.TransferTooling), TotalDelay = 0 },

                new ActivityCardViewModel
                {
                    ActivityName  = "Change Material",
                    SectionCounts = GetSectionCounts(x => x.ChangeMaterial),
                    TotalDelay    = GetDelayCount(
                        "Change Material",
                        x => x.ChangeMaterial,
                        controlNo => _dbContext.ChangeMaterialProcesses
                            .Any(x => x.ControlNumber == controlNo && x.ProcessStep == "First Delivery Date"))
                },

                //new ActivityCardViewModel { ActivityName = "New Model",      SectionCounts = GetSectionCounts(x => x.NewModel),      TotalDelay = 0 },
                //new ActivityCardViewModel { ActivityName = "Non-Concurrent", SectionCounts = GetSectionCounts(x => x.NonConcurrent), TotalDelay = 0 },

                new ActivityCardViewModel
                {
                    ActivityName  = "Other 4M",
                    SectionCounts = GetSectionCounts(x => x.Other4M),
                    TotalDelay    = GetDelayCount(
                        "Other 4M",
                        x => x.Other4M,
                        controlNo => _dbContext.Other4MProcesses
                            .Any(x => x.ControlNumber == controlNo && x.FirstDeliveryDate != null))
                }
            };

            ViewBag.TotalEntries = totalEntries;
            return View(activityCards);
        }

        // =====================================================================
        // ACTIVITY DETAILS
        // =====================================================================

        [HttpGet("ActivityDetails")]
        public IActionResult ActivityDetails(string activityName)
        {

            IQueryable<ImportData> query = _dbContext.ImportDatas;

            query = activityName switch
            {
                "Renewal / Additional Mold" => query.Where(x => x.RenewalAdditionalMold == "YES"),
                "New Tooling / Localization" => query.Where(x => x.NewToolingLocalization == "YES"),
                "Supplier Change / Localization" => query.Where(x => x.SupplierChangeLocalization == "YES"),
                "Multiple Procurement / Localization" => query.Where(x => x.MultipleProcurementLocalization == "YES"),
                "Transfer Tooling" => query.Where(x => x.TransferTooling == "YES"),
                "Change Material" => query.Where(x => x.ChangeMaterial == "YES"),
                "New Model" => query.Where(x => x.NewModel == "YES"),
                "Non-Concurrent" => query.Where(x => x.NonConcurrent == "YES"),
                "Other 4M" => query.Where(x => x.Other4M == "YES"),
                _ => query.Where(x => false)
            };

            var importDataList = query
                .OrderBy(x => x.Section)
                .ThenBy(x => x.ControlNo)
                .ToList();

            var activityCard = new ActivityCardViewModel { ActivityName = activityName };

            var model = new ActivityImportViewModel
            {
                ActivityCard = activityCard,
                ImportDataList = importDataList
            };

            return View(model);
        }

        [HttpGet("GetProcessesByActivity")]
        public IActionResult GetProcessesByActivity(string section, string activityName)
        {
            string category = activityName switch
            {
                "New Tooling / Localization" => "Localization",
                "Supplier Change / Localization" => "Supplier Change",
                "Multiple Procurement / Localization" => "Multiple Procurement",
                _ => null
            };

            if (category == null)
                return Json(new List<object>());

            var processes = _dbContext.NewToolingProcessMappings
                .Where(x => x.Category == category &&
                            x.Section.Trim().ToLower() == section.Trim().ToLower())
                .OrderBy(x => x.StepOrder)
                .Select(x => new { x.ProcessStep, x.StepOrder })
                .ToList();

            return Json(processes);
        }

        // =====================================================================
        // RENEWAL / ADDITIONAL MOLD — dropdown helpers
        // =====================================================================

        [HttpGet("GetActivitiesBySection")]
        public IActionResult GetActivitiesBySection(string section)
        {
            var activities = _dbContext.LeadTimes
                .Where(x => x.Section == section)
                .Select(x => x.Activity)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return Json(activities);
        }

        [HttpGet("GetAllowedActivities")]
        public IActionResult GetAllowedActivities(string section)
        {
            return Json(SectionActivityHelper.GetAllowedActivitiesForSection(_dbContext, section));
        }

        [HttpGet("GetActivityFields")]
        public IActionResult GetActivityFields(string section, string activity)
        {
            var matrix = _dbContext.UpdateActivityMatrices
                .FirstOrDefault(m => m.Section == section && m.Activity == activity);

            if (matrix == null) return NotFound();

            var fields = typeof(UpdateActivityMatrix)
                .GetProperties()
                .Where(p => p.PropertyType == typeof(bool))
                .Where(p => (bool)p.GetValue(matrix))
                .Select(p => p.Name)
                .ToList();

            return Ok(fields);
        }

        [HttpGet("RefreshImportData")]
        public IActionResult RefreshImportData(string section, string activity)
        {
            var importData = _dbContext.ImportDatas
                .Where(x => x.Section.Trim() == section.Trim())
                .ToList();

            var matrixRow = _dbContext.UpdateActivityMatrices
                .FirstOrDefault(x =>
                    x.Section.Trim() == section.Trim() &&
                    x.Activity.Trim() == activity.Trim());

            if (matrixRow == null)
                return Json(new { success = true, columns = new List<string>(), data = importData });

            var allowedColumns = matrixRow.GetType()
                .GetProperties()
                .Where(p =>
                    p.PropertyType == typeof(bool) &&
                    (bool)p.GetValue(matrixRow) &&
                    p.Name != "IsActive" &&
                    p.Name != "IsDeleted")
                .Select(p => p.Name)
                .ToList();

            return Json(new { success = true, columns = allowedColumns, data = importData });
        }

        // =====================================================================
        // NEW TOOLING / LOCALIZATION — dropdown helpers
        // =====================================================================

        [HttpGet("GetNewToolingCategories")]
        public IActionResult GetNewToolingCategories()
        {
            var categories = new[] { "Multiple Procurement", "Supplier Change", "Localization" };
            return Json(categories);
        }

        [HttpGet("GetNewToolingProcessesBySectionAndCategory")]
        public IActionResult GetNewToolingProcessesBySectionAndCategory(string section, string category)
        {
            var processes = _dbContext.NewToolingProcessMappings
                .Where(x => x.Section == section && x.Category == category)
                .OrderBy(x => x.StepOrder)
                .Select(x => new { x.ProcessStep, x.StepOrder })
                .ToList();

            return Json(processes);
        }

        // =====================================================================
        // CHANGE MATERIAL — dropdown helper
        // =====================================================================

        [HttpGet("GetChangeMaterialProcesses")]
        public IActionResult GetChangeMaterialProcesses(string section)
        {
            var processes = _dbContext.ChangeMaterialProcessMappings
                .Where(x => x.Section.Trim().ToLower() == section.Trim().ToLower())
                .OrderBy(x => x.StepOrder)
                .Select(x => new { x.ProcessStep, x.StepOrder })
                .ToList();

            return Json(processes);
        }

        // =====================================================================
        // RENEWAL / ADDITIONAL MOLD — GetPartial + private handlers
        // =====================================================================

        [HttpGet("GetPartial")]
        public async Task<IActionResult> GetPartial(string section, string activity, string process)
        {
            IQueryable<ImportData> query = _dbContext.ImportDatas;

            query = activity switch
            {
                "Renewal / Additional Mold" => query.Where(x => x.RenewalAdditionalMold == "YES"),
                "New Tooling / Localization" => query.Where(x => x.NewToolingLocalization == "YES"),
                "Transfer Tooling" => query.Where(x => x.TransferTooling == "YES"),
                "New Model" => query.Where(x => x.NewModel == "YES"),
                "Non-Concurrent" => query.Where(x => x.NonConcurrent == "YES"),
                "Supplier Change / Localization" => query.Where(x => x.SupplierChangeLocalization == "YES"),
                "Other 4M" => query.Where(x => x.Other4M == "YES"),
                _ => query.Where(x => false)
            };

            return process switch
            {
                "Tooling Quotation Request~Approval" => await HandleToolingQuotationRequestApproval(query, process, section),
                "Tooling Request-Order" => await HandleToolingRequestOrder(query, process, section),
                "Tooling PO Issuance" => await HandleToolingPoIssuance(query, process, section),
                "DFM/QCD Approval" => await HandleDFMQCDApproval(query, process, section),
                "Tooling Fabrication" => await HandleToolingFabrication(query, process, section),
                "Tooling Transfer (Arrival in PH)" => await HandleToolingTransfer(query, process, section),
                "Kataken Submission (Local Trial)" => await HandleKatakenSubmission(query, process, section),
                "Kataken Finish (Local Trial)" => await HandleKatakenFinish(query, process, section),
                "DE Evaluation" => await HandleEvaluation(query, process, section),
                "QA Special Evaluation" => await HandleSpecialEvaluation(query, process, section),
                "Test Run" => await HandleTestRun(query, process, section),
                _ => Content("<div class='text-muted'>No process partial defined</div>")
            };
        }

        // NOTE: All Handle* methods below now call ComputeLimitAndRemaining(...) right after
        // mapping the raw ImportData rows into their VM, so LimitDate/RemainingDays are
        // populated before the PartialView renders. Activity name passed is "Renewal / Additional
        // Mold" to match the LeadTimes table lookup (Section + Activity), consistent with how
        // the corresponding Save* actions on this controller tag these steps as ActivityType = "Renewal".

        private async Task<IActionResult> HandleToolingQuotationRequestApproval(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.ToolingQuotationRequestApproval.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapQuotationRequest(a)).ToList();

            // Use the processing department's section (e.g. "MP2") for the LeadTimes lookup,
            // NOT vm.Section — that's the part's origin section (e.g. "IQC") and won't match
            // any row in LeadTimes for this activity.
            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/MP2/_MP2ToolingQuotationRequestApproval", vms);
        }

        private async Task<IActionResult> HandleToolingRequestOrder(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.MP2ToolingRequestOrder.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapRequestOrder(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/MP2/_MP2ToolingRequestOrder", vms);
        }

        private async Task<IActionResult> HandleToolingPoIssuance(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.MP2ToolingPoIssuance.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapPoIssuance(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/MP2/_MP2ToolingPOIssuance", vms);
        }

        private async Task<IActionResult> HandleDFMQCDApproval(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.SQCDFMQCDApprovals.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapDfmQcdApproval(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/SQC/_SQCDFM_QCDApproval", vms);
        }

        private async Task<IActionResult> HandleToolingFabrication(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.MP2ToolingFabrications.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapToolingFabrication(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/MP2/_MP2ToolingFabrication", vms);
        }

        private async Task<IActionResult> HandleToolingTransfer(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.MP2ToolingTransfers.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapToolingTransfer(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/MP2/_MP2ToolingTransfer", vms);
        }

        private async Task<IActionResult> HandleKatakenSubmission(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.IQCKatakenSubmissions.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapKatakenSubmission(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/IQC/_IQCKatakenSubmission", vms);
        }

        private async Task<IActionResult> HandleKatakenFinish(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.IQCKatakenFinish.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapKatakenFinish(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/IQC/_IQCKatakenFinish", vms);
        }

        private async Task<IActionResult> HandleEvaluation(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.DEEvaluation.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapEvaluation(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/DE/_DEEvaluation", vms);
        }

        private async Task<IActionResult> HandleSpecialEvaluation(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.QASpecialEvaluations.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapSpecialEvaluation(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/QA/_QASpecialEvaluation", vms);
        }

        private async Task<IActionResult> HandleTestRun(IQueryable<ImportData> query, string process, string section)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.IQCTestRuns.Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    && _dbContext.ActivityCurrentProcesses.Any(acp => acp.ControlNumber == importData.ControlNo && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content("<div class='alert alert-warning text-center mt-3'><i class='fa-solid fa-triangle-exclamation me-2'></i>No imported data for this process yet.</div>");

            var vms = list.Select(a => _updateActivityMapperService.MapTestRun(a)).ToList();

            ComputeLimitAndRemaining(
                vms,
                vm => vm.ControlNo,
                vm => section,
                process,      // LeadTimes.Activity actually stores the process-step name
                process);

            return PartialView("Partials/IQC/_IQCTestRun", vms);
        }

        // =====================================================================
        // NEW TOOLING / LOCALIZATION — GetPartial + Save
        // =====================================================================

        [HttpGet("GetNewToolingPartial")]
        public async Task<IActionResult> GetNewToolingPartial(
            string section, string activityName, string process)
        {
            if (string.IsNullOrWhiteSpace(section) ||
                string.IsNullOrWhiteSpace(activityName) ||
                string.IsNullOrWhiteSpace(process))
                return Content("<div class='alert alert-warning'>Section, Activity, and Process are required.</div>");

            string category = activityName switch
            {
                "New Tooling / Localization" => "Localization",
                "Supplier Change / Localization" => "Supplier Change",
                "Multiple Procurement / Localization" => "Multiple Procurement",
                _ => null
            };

            if (category == null)
                return Content("<div class='alert alert-warning'>Unknown activity.</div>");

            IQueryable<ImportData> baseQuery = _dbContext.ImportDatas;
            baseQuery = activityName switch
            {
                "New Tooling / Localization" => baseQuery.Where(x => x.NewToolingLocalization == "YES"),
                "Supplier Change / Localization" => baseQuery.Where(x => x.SupplierChangeLocalization == "YES"),
                "Multiple Procurement / Localization" => baseQuery.Where(x => x.MultipleProcurementLocalization == "YES"),
                _ => baseQuery.Where(x => false)
            };

            var mapping = _dbContext.NewToolingProcessMappings
                .FirstOrDefault(x =>
                    x.Category == category &&
                    x.ProcessStep.Trim().ToLower() == process.Trim().ToLower());

            if (mapping == null)
                return Content("<div class='alert alert-warning'>Process mapping not found.</div>");

            var nextMapping = _dbContext.NewToolingProcessMappings
                .Where(x => x.Category == category && x.StepOrder > mapping.StepOrder)
                .OrderBy(x => x.StepOrder)
                .FirstOrDefault();

            string nextProcess = nextMapping?.ProcessStep ?? "Completed";

            var list = await baseQuery
                .Where(importData =>
                    !_dbContext.NewToolingLocalizationProcesses
                        .Any(p =>
                            p.ControlNumber == importData.ControlNo &&
                            p.Category == category &&
                            p.ProcessStep == process) &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content(@"
            <div class='alert alert-warning text-center mt-3'>
                <i class='fa-solid fa-triangle-exclamation me-2'></i>
                No imported data for this process yet.
            </div>");

            var viewModels = list.Select(importData => new NewToolingLocalizationProcessVM
            {
                ControlNumber = importData.ControlNo,
                Section = importData.Section,
                ToolingType = importData.ToolingType,
                ToolingCategory = importData.ToolingCategory,
                Model = importData.Model,
                ChildPartcode = importData.ChildPartcode,
                PartName = importData.PartName,
                ToolingManagement = importData.ToolingManagement,
                Supplier = importData.Supplier,
                BIPHMoldNo = importData.BIPHMoldNo,
                SupplierMoldNo = importData.SupplierMoldNo,
                MoldMaker = importData.MoldMaker,
                Category = category,
                ActivityName = activityName,
                ProcessStep = process,
                StepOrder = mapping.StepOrder,
                NextProcessStep = nextProcess
            }).ToList();

            // NOTE: NewToolingLocalizationProcessVM does not currently inherit BasedImportData,
            // so ComputeLimitAndRemaining(...) is not wired here. If you want Limit
            // Date/Remaining Days on this screen too, add LimitDate/RemainingDays properties
            // to NewToolingLocalizationProcessVM (or have it inherit BasedImportData) and share
            // its VM class definition so the call can be added with the correct property
            // accessors — same as was done for the MP2 handlers above.

            return PartialView("Partials/NewTooling/_NewToolingLocalizationProcess", viewModels);
        }

        [HttpPost("SaveNewToolingLocalizationProcess")]
        public IActionResult SaveNewToolingLocalizationProcess(
            [FromBody] SaveNewToolingLocalizationProcessDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string category = dto.ActivityName switch
                {
                    "New Tooling / Localization" => "Localization",
                    "Supplier Change / Localization" => "Supplier Change",
                    "Multiple Procurement / Localization" => "Multiple Procurement",
                    _ => dto.Category
                };

                var existingCategory = _dbContext.NewToolingCategories
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (existingCategory == null)
                {
                    _dbContext.NewToolingCategories.Add(new NewToolingCategory
                    {
                        ControlNumber = dto.ControlNumber,
                        Category = category,
                        AssignedAt = DateTime.UtcNow,
                        AssignedBy = User.Identity?.Name ?? "SYSTEM"
                    });
                }

                var nextMapping = _dbContext.NewToolingProcessMappings
                    .Where(x => x.Category == category && x.StepOrder > dto.StepOrder)
                    .OrderBy(x => x.StepOrder)
                    .FirstOrDefault();

                string nextProcess = nextMapping?.ProcessStep ?? "Completed";

                DateTime? finalPOIssuedDate = !string.IsNullOrWhiteSpace(dto.FinalPOIssuedDate)
                    ? DateTime.Parse(dto.FinalPOIssuedDate).ToUniversalTime()
                    : null;

                var entity = _dbContext.NewToolingLocalizationProcesses
                    .FirstOrDefault(x =>
                        x.ControlNumber == dto.ControlNumber &&
                        x.Category == category &&
                        x.ProcessStep == dto.ProcessStep);

                if (entity == null)
                {
                    entity = new NewToolingLocalizationProcess
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = dto.ActivityName,
                        Category = category,
                        ProcessStep = dto.ProcessStep,
                        StepOrder = dto.StepOrder,
                        TargetDate = ToUtc(dto.TargetDate),
                        ActualDate = ToUtc(dto.ActualDate),
                        Result = dto.Result,
                        ReferenceNo = dto.ReferenceNo,
                        Remarks = dto.Remarks ?? string.Empty,
                        FinalPOIssuedDate = finalPOIssuedDate,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = nextProcess
                    };
                    _dbContext.NewToolingLocalizationProcesses.Add(entity);
                }
                else
                {
                    entity.TargetDate = ToUtc(dto.TargetDate);
                    entity.ActualDate = ToUtc(dto.ActualDate);
                    entity.Result = dto.Result;
                    entity.ReferenceNo = dto.ReferenceNo;
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.FinalPOIssuedDate = finalPOIssuedDate;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = nextProcess;
                }

                // SaveNewToolingLocalizationProcess — activity type comes from dto.ActivityName
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = nextProcess,
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = dto.ActivityName switch  // ← add
                    {
                        "New Tooling / Localization" => "Localization",
                        "Supplier Change / Localization" => "SupplierChange",
                        "Multiple Procurement / Localization" => "MultipleProcurement",
                        _ => dto.ActivityName
                    }
                });

                WriteTransactionLog(
                    importData,
                    dto.ActivityName ?? category,
                    dto.ProcessStep,
                    nextProcess,
                    User.Identity?.Name ?? "SYSTEM",
                    dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // =====================================================================
        // CHANGE MATERIAL — GetPartial + Save
        // =====================================================================

        [HttpGet("GetChangeMaterialPartial")]
        public async Task<IActionResult> GetChangeMaterialPartial(string process)
        {
            if (string.IsNullOrWhiteSpace(process))
                return Content("<div class='alert alert-warning'>Process is required.</div>");

            var mapping = _dbContext.ChangeMaterialProcessMappings
                .FirstOrDefault(x => x.ProcessStep.Trim().ToLower() == process.Trim().ToLower());

            if (mapping == null)
                return Content("<div class='alert alert-warning'>Process mapping not found.</div>");

            var nextMapping = _dbContext.ChangeMaterialProcessMappings
                .Where(x => x.StepOrder > mapping.StepOrder)
                .OrderBy(x => x.StepOrder)
                .FirstOrDefault();

            // Last step stays as "First Delivery Date" (no "Completed" string)
            string nextProcess = nextMapping?.ProcessStep ?? "First Delivery Date";

            var list = await _dbContext.ImportDatas
                .Where(x => x.ChangeMaterial == "YES")
                .Where(importData =>
                    !_dbContext.ChangeMaterialProcesses
                        .Any(p => p.ControlNumber == importData.ControlNo
                               && p.ProcessStep == process) &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp => acp.ControlNumber == importData.ControlNo
                                 && acp.CurrentProcess == process))
                .ToListAsync();

            if (!list.Any())
                return Content(@"
                    <div class='alert alert-warning text-center mt-3'>
                        <i class='fa-solid fa-triangle-exclamation me-2'></i>
                        No imported data for this process yet.
                    </div>");

            var viewModels = list.Select(importData => new ChangeMaterialProcessVM
            {
                ControlNumber = importData.ControlNo,
                Section = importData.Section,
                Model = importData.Model,
                ChildPartcode = importData.ChildPartcode,
                PartName = importData.PartName,
                Supplier = importData.Supplier,
                BIPHMoldNo = importData.BIPHMoldNo,
                SupplierMoldNo = importData.SupplierMoldNo,
                MoldMaker = importData.MoldMaker,
                ToolingManagement = importData.ToolingManagement,
                ProcessStep = process,
                StepOrder = mapping.StepOrder,
                NextProcessStep = nextProcess
            }).ToList();

            // NOTE: same as GetNewToolingPartial above — ChangeMaterialProcessVM does not
            // currently inherit BasedImportData, so LimitDate/RemainingDays are not wired
            // here. Share the VM if you want this screen included too.

            return PartialView("Partials/ChangeMaterial/_ChangeMaterialProcess", viewModels);
        }

        [HttpPost("SaveChangeMaterialProcess")]
        public IActionResult SaveChangeMaterialProcess([FromBody] SaveChangeMaterialProcessDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                var mapping = _dbContext.ChangeMaterialProcessMappings
                    .FirstOrDefault(x => x.ProcessStep == dto.ProcessStep);

                if (mapping == null)
                    return NotFound(new { success = false, message = "Process mapping not found" });

                var nextMapping = _dbContext.ChangeMaterialProcessMappings
                    .Where(x => x.StepOrder > mapping.StepOrder)
                    .OrderBy(x => x.StepOrder)
                    .FirstOrDefault();

                // Last step stays as "First Delivery Date"
                string nextProcess = nextMapping?.ProcessStep ?? "First Delivery Date";

                var entity = _dbContext.ChangeMaterialProcesses
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber
                                      && x.ProcessStep == dto.ProcessStep);

                if (entity == null)
                {
                    entity = new ChangeMaterialProcess
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = "Change Material",
                        ProcessStep = dto.ProcessStep,
                        StepOrder = mapping.StepOrder,
                        TargetDate = ToUtc(dto.TargetDate),
                        ActualDate = ToUtc(dto.ActualDate),
                        Remarks = dto.Remarks ?? string.Empty,
                        CurrentProcess = nextProcess,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow
                    };
                    _dbContext.ChangeMaterialProcesses.Add(entity);
                }
                else
                {
                    entity.TargetDate = ToUtc(dto.TargetDate);
                    entity.ActualDate = ToUtc(dto.ActualDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.CurrentProcess = nextProcess;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                // SaveChangeMaterialProcess
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = nextProcess,
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "ChangeMaterial"   // ← add
                });

                WriteTransactionLog(
                    importData,
                    "Change Material",
                    dto.ProcessStep,
                    nextProcess,
                    User.Identity?.Name ?? "SYSTEM",
                    dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // =====================================================================
        // RENEWAL / ADDITIONAL MOLD — Save Actions
        // =====================================================================

        [HttpPost("SaveMP2ToolingQuotationRequestApprovalUpdate")]
        public IActionResult SaveMP2ToolingQuotationRequestApprovalUpdate(
            [FromBody] SaveMP2ToolingQuotationRequestApprovalVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.ToolingQuotationRequestApproval
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new MP2_ToolingQuotationRequestApproval
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        TargetDate = ToUtc(dto.TargetDate),
                        ActualDate = ToUtc(dto.ActualDate),
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        Process = "Tooling Quotation Request~Approval",
                        CurrentProcess = "Tooling Request-Order"
                    };
                    _dbContext.ToolingQuotationRequestApproval.Add(entity);
                }
                else
                {
                    entity.TargetDate = ToUtc(dto.TargetDate);
                    entity.ActualDate = ToUtc(dto.ActualDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Tooling Request-Order";
                }

                // SaveMP2ToolingQuotationRequestApprovalUpdate
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "Tooling Request-Order",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← add
                });

                WriteTransactionLog(importData, activity,
                    "Tooling Quotation Request~Approval",
                    "Tooling Request-Order",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveMP2ActivityUpdate")]
        public IActionResult SaveMP2ActivityUpdate([FromBody] SaveMP2ActivityUpdateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.MP2ToolingRequestOrder
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new MP2_ToolingRequestOrder
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        TRFNo = dto.TRFNo ?? "N/A",
                        TargetDate = ToUtc(dto.TargetDate),
                        ActualDate = ToUtc(dto.ActualDate),
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Tooling PO Issuance"
                    };
                    _dbContext.MP2ToolingRequestOrder.Add(entity);
                }
                else
                {
                    entity.TRFNo = dto.TRFNo ?? string.Empty;
                    entity.TargetDate = ToUtc(dto.TargetDate);
                    entity.ActualDate = ToUtc(dto.ActualDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Tooling PO Issuance";
                }

                // SaveMP2ActivityUpdate
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "Tooling PO Issuance",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← add
                });

                WriteTransactionLog(importData, activity,
                    "Tooling Request-Order",
                    "Tooling PO Issuance",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveMP2ToolingPoIssuance")]
        public IActionResult SaveMP2ToolingPoIssuance([FromBody] SaveMP2ToolingPoIssuanceVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.MP2ToolingPoIssuance
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new MP2_ToolingPoIssuance
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        TargetIssueDate = ToUtc(dto.TargetIssueDate),
                        ActualIssueDate = ToUtc(dto.ActualIssueDate),
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "DFM/QCD Approval"
                    };
                    _dbContext.MP2ToolingPoIssuance.Add(entity);
                }
                else
                {
                    entity.TargetIssueDate = ToUtc(dto.TargetIssueDate);
                    entity.ActualIssueDate = ToUtc(dto.ActualIssueDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "DFM/QCD Approval";
                }

                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "DFM/QCD Approval",   // ✅ fixed
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"
                });

                WriteTransactionLog(importData, activity,
                    "Tooling PO Issuance",
                    "DFM/QCD Approval",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveSQCDfmQcdApproval")]
        public IActionResult SaveSQCDfmQcdApproval([FromBody] SaveSQCDfmQcdApprovalVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.SQCDFMQCDApprovals
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new SQC_DFMQCDApproval
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        NeedNoNeedQcdMtg = dto.NeedNoNeedQcdMtg,
                        ApprovalLeadTime = dto.ApprovalLeadTime,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Tooling Fabrication"
                    };
                    _dbContext.SQCDFMQCDApprovals.Add(entity);
                }
                else
                {
                    entity.NeedNoNeedQcdMtg = dto.NeedNoNeedQcdMtg;
                    entity.ApprovalLeadTime = dto.ApprovalLeadTime;
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Tooling Fabrication";
                }

                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "Tooling Fabrication",
                    UpdateAt = DateTime.UtcNow
                });

                WriteTransactionLog(importData, activity,
                    "DFM/QCD Approval",
                    "Tooling Fabrication",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveMP2ToolingFabrication")]
        public IActionResult SaveMP2ToolingFabrication([FromBody] SaveToolingFabricationVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.MP2ToolingFabrications
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new MP2_ToolingFabrication
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        FabricationLeadTime = dto.FabricationLeadTime,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Tooling Transfer (Arrival in PH)"
                    };
                    _dbContext.MP2ToolingFabrications.Add(entity);
                }
                else
                {
                    entity.FabricationLeadTime = dto.FabricationLeadTime;
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Tooling Transfer (Arrival in PH)";
                }

                // SaveMP2ToolingFabrication
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "Tooling Transfer (Arrival in PH)",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← add
                });

                WriteTransactionLog(importData, activity,
                    "Tooling Fabrication",
                    "Tooling Transfer (Arrival in PH)",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveMP2ToolingTransfer")]
        public IActionResult SaveMP2ToolingTransfer([FromBody] SaveToolingTransferVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.MP2ToolingTransfers
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new MP2_ToolingTransfer
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        TransferLeadTime = dto.TransferLeadTime,
                        TargetArrivalDate = ToUtc(dto.TargetArrivalDate),
                        ActualArrivalDate = ToUtc(dto.ActualArrivalDate),
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Kataken Submission (Local Trial)"
                    };
                    _dbContext.MP2ToolingTransfers.Add(entity);
                }
                else
                {
                    entity.TransferLeadTime = dto.TransferLeadTime;
                    entity.TargetArrivalDate = ToUtc(dto.TargetArrivalDate);
                    entity.ActualArrivalDate = ToUtc(dto.ActualArrivalDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Kataken Submission (Local Trial)";
                }

                // SaveMP2ToolingTransfer
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "Kataken Submission (Local Trial)",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← add
                });

                WriteTransactionLog(importData, activity,
                    "Tooling Transfer (Arrival in PH)",
                    "Kataken Submission (Local Trial)",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveIQCKatakenSubmission")]
        public IActionResult SaveIQCKatakenSubmission([FromBody] SaveKatakenSubmissionVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.IQCKatakenSubmissions
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new IQC_KatakenSubmission
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        ActualSubmissionDate = ToUtc(dto.ActualSubmissionDate),
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Kataken Finish (Local Trial)"
                    };
                    _dbContext.IQCKatakenSubmissions.Add(entity);
                }
                else
                {
                    entity.ActualSubmissionDate = ToUtc(dto.ActualSubmissionDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Kataken Finish (Local Trial)";
                }

                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "Kataken Finish (Local Trial)",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← fix
                });

                WriteTransactionLog(importData, activity,
                    "Kataken Submission (Local Trial)",
                    "Kataken Finish (Local Trial)",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveIQCKatakenFinish")]
        public IActionResult SaveIQCKatakenFinish([FromBody] SaveKatakenFinishVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.IQCKatakenFinish
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new IQC_KatakenFinish
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Result = dto.Result,
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "DE Evaluation"
                    };
                    _dbContext.IQCKatakenFinish.Add(entity);
                }
                else
                {
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Result = dto.Result;
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "DE Evaluation";
                }

                // SaveIQCKatakenFinish
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "DE Evaluation",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← add
                });

                WriteTransactionLog(importData, activity,
                    "Kataken Finish (Local Trial)",
                    "DE Evaluation",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveDEEvaluation")]
        public IActionResult SaveDEEvaluation([FromBody] SaveEvaluationVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.DEEvaluation
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new DE_Evaluation
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        NeedNoNeed = dto.NeedNoNeed,
                        LeadTime = dto.LeadTime,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Result = dto.Result,
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "QA Special Evaluation"
                    };
                    _dbContext.DEEvaluation.Add(entity);
                }
                else
                {
                    entity.NeedNoNeed = dto.NeedNoNeed;
                    entity.LeadTime = dto.LeadTime;
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Result = dto.Result;
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "QA Special Evaluation";
                }

                // SaveDEEvaluation
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "QA Special Evaluation",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← add
                });

                WriteTransactionLog(importData, activity,
                    "DE Evaluation",
                    "QA Special Evaluation",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveQASpecialEvaluation")]
        public IActionResult SaveQASpecialEvaluation([FromBody] SaveSpecialEvaluationDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.QASpecialEvaluations
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new QA_SpecialEvaluation
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        NeedNoNeed = dto.NeedNoNeed,
                        LeadTime = dto.LeadTime,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Result = dto.Result,
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Test Run"
                    };
                    _dbContext.QASpecialEvaluations.Add(entity);
                }
                else
                {
                    entity.NeedNoNeed = dto.NeedNoNeed;
                    entity.LeadTime = dto.LeadTime;
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Result = dto.Result;
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Test Run";
                }

                // SaveQASpecialEvaluation
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "Test Run",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← add
                });

                WriteTransactionLog(importData, activity,
                    "QA Special Evaluation",
                    "Test Run",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("SaveIQCTestRun")]
        public IActionResult SaveIQCTestRun([FromBody] SaveTestRunDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                string activity = DetermineActivity(importData);

                var entity = _dbContext.IQCTestRuns
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new IQC_TestRun
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        ResultEmailDatetoSupplier = ToUtc(dto.ResultEmailDatetoSupplier),
                        ResultPassedFailed = dto.ResultPassedFailed,
                        Remarks = dto.Remarks ?? string.Empty,
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "MP2-PDC"
                    };
                    _dbContext.IQCTestRuns.Add(entity);
                }
                else
                {
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.ResultEmailDatetoSupplier = ToUtc(dto.ResultEmailDatetoSupplier);
                    entity.ResultPassedFailed = dto.ResultPassedFailed;
                    entity.Remarks = dto.Remarks ?? string.Empty;
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "MP2-PDC";
                }

                // SaveIQCTestRun
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = "MP2-PDC",
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Renewal"   // ← add
                });

                WriteTransactionLog(importData, activity,
                    "Test Run",
                    "MP2-PDC",
                    User.Identity?.Name ?? "SYSTEM", dto.Remarks);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // =====================================================================
        // CAPA PDC
        // =====================================================================

        [HttpPost("SaveMp2CapaPdc")]
        public async Task<IActionResult> SaveMp2CapaPdc([FromBody] MP2CapaPdcDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("DTO is null");

                if (string.IsNullOrEmpty(dto.ControlNumber))
                    return BadRequest("ControlNumber is required");

                var record = await _dbContext.MP2_Capa_PDCs
                    .FirstOrDefaultAsync(x => x.ControlNumber == dto.ControlNumber);

                if (record == null)
                {
                    record = new MP2_Capa_PDC
                    {
                        ControlNumber = dto.ControlNumber,
                        PartsShortageDate = ToUtc(dto.PartsShortageDate),
                        TargetMoldUsageDate = ToUtc(dto.TargetMoldUsageDate),
                        InputBy = User.Identity?.Name ?? "System",
                        CreateDate = DateTime.UtcNow
                    };
                    _dbContext.MP2_Capa_PDCs.Add(record);
                }
                else
                {
                    record.PartsShortageDate = ToUtc(dto.PartsShortageDate);
                    record.TargetMoldUsageDate = ToUtc(dto.TargetMoldUsageDate);
                }

                await _dbContext.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        // =====================================================================
        // PRIVATE HELPERS
        // =====================================================================

        private DateTime? ToUtc(DateTime? dateTime)
        {
            if (!dateTime.HasValue) return null;
            return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
        }

        private string DetermineActivity(ImportData data)
        {
            if (data.RenewalAdditionalMold == "YES") return "Renewal / Additional Mold";
            if (data.NewToolingLocalization == "YES") return "New Tooling / Localization";
            if (data.SupplierChangeLocalization == "YES") return "Supplier Change / Localization";
            if (data.MultipleProcurementLocalization == "YES") return "Multiple Procurement / Localization";
            if (data.TransferTooling == "YES") return "Transfer Tooling";
            if (data.ChangeMaterial == "YES") return "Change Material";
            if (data.NewModel == "YES") return "New Model";
            if (data.NonConcurrent == "YES") return "Non-Concurrent";
            if (data.Other4M == "YES") return "Other 4M";
            return "Unknown";
        }

        // =====================================================================
        // SHARED: Section → Allowed Activities (based on actual process-step ownership)
        // =====================================================================
        private List<string> GetAllowedActivitiesForSection(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
                return new List<string>();

            var allowed = new List<string>();

            if (_dbContext.LeadTimes.Any(x => x.Section == section))
                allowed.Add("Renewal / Additional Mold");

            if (_dbContext.NewToolingProcessMappings.Any(x => x.Category == "Localization" && x.Section == section))
                allowed.Add("New Tooling / Localization");

            if (_dbContext.NewToolingProcessMappings.Any(x => x.Category == "Supplier Change" && x.Section == section))
                allowed.Add("Supplier Change / Localization");

            if (_dbContext.NewToolingProcessMappings.Any(x => x.Category == "Multiple Procurement" && x.Section == section))
                allowed.Add("Multiple Procurement / Localization");

            if (_dbContext.ChangeMaterialProcessMappings.Any(x => x.Section == section))
                allowed.Add("Change Material");

            // Other4MProcessMappings has no Section column — hardcoded to IQC in the view
            if (string.Equals(section, "IQC", StringComparison.OrdinalIgnoreCase))
                allowed.Add("Other 4M");

            return allowed;
        }

        private void WriteTransactionLog(
            ImportData importData,
            string activity,
            string currentProcess,
            string nextProcess,
            string inputBy,
            string remarks = "")
        {
            var previousLog = _dbContext.TransactionLogs
                .Where(x => x.TransactionNumber == importData.ControlNo
                         && x.Activity == activity
                         && x.CurrentProcess == currentProcess)
                .OrderByDescending(x => x.InputDate)
                .FirstOrDefault();

            if (previousLog != null)
                previousLog.Remarks = remarks ?? string.Empty;

            // Terminal step being re-saved (e.g. entering the actual First Delivery
            // Date value) — currentProcess and nextProcess are the same, so the
            // existing row above already represents this state. Don't duplicate it.
            if (previousLog != null && currentProcess == nextProcess)
                return;

            _dbContext.TransactionLogs.Add(new TransactionLogs
            {
                TransactionNumber = importData.ControlNo,
                PartName = importData.PartName,
                Supplier = importData.Supplier,
                Model = importData.Model,
                Activity = activity,
                Source = "Update Activity",
                PIC = inputBy,
                StartDate = importData.DateImported,
                EndDate = DateTime.UtcNow,
                ReceivedDate = DateTime.UtcNow,
                InputDate = DateTime.UtcNow,
                CurrentProcess = nextProcess,
                Status = nextProcess == "First Delivery Date" ? "Completed"
                                  : nextProcess == "FIRST DELIVERY DATE" ? "Completed"
                                  : nextProcess == "Completed" ? "Completed"
                                  : "In Progress",
                Remarks = string.Empty
            });
        }


        // =====================================================================
        // OTHER 4M — GetPartial + Save  (single wide table, 13 fixed steps)
        // =====================================================================

        [HttpGet("GetOther4MPartial")]
        public async Task<IActionResult> GetOther4MPartial(string process)
        {
            var section = User.FindFirst("Section")?.Value;
            if (!string.Equals(section, "IQC", StringComparison.OrdinalIgnoreCase))
            {
                return Content("<div class='alert alert-danger text-center mt-3'><i class='fa-solid fa-lock me-2'></i>Your section does not have access to this process.</div>");
            }
            if (string.IsNullOrWhiteSpace(process))
                return Content("<div class='alert alert-warning'>Process is required.</div>");

            var mapping = _dbContext.Other4MProcessMappings
                .FirstOrDefault(x => x.ProcessStep.Trim().ToLower() == process.Trim().ToLower());

            if (mapping == null)
                return Content("<div class='alert alert-warning'>Process mapping not found.</div>");

            var nextMapping = _dbContext.Other4MProcessMappings
        .Where(x => x.StepOrder > mapping.StepOrder)
        .OrderBy(x => x.StepOrder)
        .FirstOrDefault();

            // nextProcess = ginagamit pa rin internally (routing/log) — keep as is
            string nextProcess = nextMapping?.ProcessStep ?? "FIRST DELIVERY DATE";

            // para lang sa display sa partial view
            string displayNextStep = nextMapping?.ProcessStep ?? "Completed";
            var list = await _dbContext.ImportDatas
                .Where(x => x.Other4M == "YES")
                .Where(importData =>
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp => acp.ControlNumber == importData.ControlNo
                                 && acp.CurrentProcess == process)
                    &&
                    !_dbContext.Other4MProcesses
                        .Any(o4m => o4m.ControlNumber == importData.ControlNo
                                 && o4m.StepOrder > mapping.StepOrder))
                .ToListAsync();

            if (!list.Any())
                return Content(@"
                    <div class='alert alert-warning text-center mt-3'>
                        <i class='fa-solid fa-triangle-exclamation me-2'></i>
                        No imported data for this process yet.
                    </div>");

            var viewModels = list.Select(importData => new Other4MProcessVM
            {
                ControlNumber = importData.ControlNo,
                Section = importData.Section,
                Model = importData.Model,
                ChildPartcode = importData.ChildPartcode,
                PartName = importData.PartName,
                Supplier = importData.Supplier,
                BIPHMoldNo = importData.BIPHMoldNo,
                SupplierMoldNo = importData.SupplierMoldNo,
                MoldMaker = importData.MoldMaker,
                ToolingManagement = importData.ToolingManagement,
                ReasonOfChange = importData.ReasonOfChange,
                ProcessStep = process,
                StepOrder = mapping.StepOrder,
                NextProcessStep = displayNextStep
            }).ToList();

            // NOTE: same as GetNewToolingPartial/GetChangeMaterialPartial above —
            // Other4MProcessVM does not currently inherit BasedImportData, so
            // LimitDate/RemainingDays are not wired here.

            return PartialView("Partials/Other4M/_Other4MProcess", viewModels);
        }

        [HttpGet("GetOther4MProcesses")]
        public IActionResult GetOther4MProcesses()
        {
            var section = User.FindFirst("Section")?.Value;
            if (!string.Equals(section, "IQC", StringComparison.OrdinalIgnoreCase))
                return Json(new List<object>());
            var processes = _dbContext.Other4MProcessMappings
                .OrderBy(x => x.StepOrder)
                .Select(x => new { x.ProcessStep, x.StepOrder })
                .ToList();

            return Json(processes);
        }

        [HttpPost("SaveOther4MProcess")]
        public IActionResult SaveOther4MProcess([FromBody] SaveOther4MProcessDto dto)
        {
            var section = User.FindFirst("Section")?.Value;
            if (!string.Equals(section, "IQC", StringComparison.OrdinalIgnoreCase))
                return StatusCode(403, new { success = false, message = "Your section does not have access to Other 4M." });

            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber) || string.IsNullOrWhiteSpace(dto.ProcessStep))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var importData = _dbContext.ImportDatas
                    .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                if (importData == null)
                    return NotFound(new { success = false, message = "Control number not found" });

                var mapping = _dbContext.Other4MProcessMappings
                    .FirstOrDefault(x => x.ProcessStep == dto.ProcessStep);

                if (mapping == null)
                    return NotFound(new { success = false, message = "Process mapping not found" });

                var nextMapping = _dbContext.Other4MProcessMappings
                    .Where(x => x.StepOrder > mapping.StepOrder)
                    .OrderBy(x => x.StepOrder)
                    .FirstOrDefault();

                string nextProcess = nextMapping?.ProcessStep ?? "FIRST DELIVERY DATE";

                var entity = _dbContext.Other4MProcesses
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    entity = new Other4MProcess
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section ?? "IQC",
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow
                    };
                    _dbContext.Other4MProcesses.Add(entity);
                }

                // ── Apply only the fields relevant to the step being saved ──
                switch (dto.ProcessStep)
                {
                    case "Test Run meeting date":
                        entity.TestRunMeetingTargetDate = ToUtc(dto.TestRunMeetingTargetDate);
                        entity.TestRunMeetingActualDate = ToUtc(dto.TestRunMeetingActualDate);
                        break;

                    case "Kataken Request date":
                        entity.KatakenRequestTargetDate = ToUtc(dto.KatakenRequestTargetDate);
                        entity.KatakenRequestActualDate = ToUtc(dto.KatakenRequestActualDate);
                        break;

                    case "Kataken PH Sample Submission":
                        entity.KatakenSampleSubmissionDate = ToUtc(dto.KatakenSampleSubmissionDate);
                        break;

                    case "Kataken Evaluation Approval":
                        entity.KatakenRequestedDate = ToUtc(dto.KatakenRequestedDate);
                        entity.KatakenSubmissionDate = ToUtc(dto.KatakenSubmissionDate);
                        entity.KatakenApprovedDate = ToUtc(dto.KatakenApprovedDate);
                        entity.KatakenStatus = dto.KatakenStatus;
                        entity.KatakenRemarks = dto.KatakenRemarks ?? string.Empty;
                        break;

                    case "DE Evaluation":
                        entity.DEReferenceNo = dto.DEReferenceNo;
                        entity.DEWorkflowSystemNo = dto.DEWorkflowSystemNo;
                        entity.DEPartsReceivedDate = ToUtc(dto.DEPartsReceivedDate);
                        entity.DEPartsEndorsementDate = ToUtc(dto.DEPartsEndorsementDate);
                        entity.DEActualFinishedDate = ToUtc(dto.DEActualFinishedDate);
                        entity.DEEvalStatus = dto.DEEvalStatus;
                        entity.DERemarks = dto.DERemarks ?? string.Empty;
                        break;

                    case "EE Evaluation":
                        entity.EEPartsReceivedDate = ToUtc(dto.EEPartsReceivedDate);
                        entity.EEPartsEndorsementDate = ToUtc(dto.EEPartsEndorsementDate);
                        entity.EEActualFinishedDate = ToUtc(dto.EEActualFinishedDate);
                        entity.EEEvalStatus = dto.EEEvalStatus;
                        entity.EERemarks = dto.EERemarks ?? string.Empty;
                        break;

                    case "QA Evaluation":
                        entity.QAWorkflowNo = dto.QAWorkflowNo;
                        entity.QATargetDeliveryDate = ToUtc(dto.QATargetDeliveryDate);
                        entity.QAPartsReceivedDate = ToUtc(dto.QAPartsReceivedDate);
                        entity.QAPartsEndorsementDate = ToUtc(dto.QAPartsEndorsementDate);
                        entity.QAActualFinishedDate = ToUtc(dto.QAActualFinishedDate);
                        entity.QAEvalStatus = dto.QAEvalStatus;
                        entity.QARemarks = dto.QARemarks ?? string.Empty;
                        break;

                    case "ITF Process":
                        entity.ITFActualFinishedDate = ToUtc(dto.ITFActualFinishedDate);
                        entity.ITFStatus = dto.ITFStatus;
                        entity.ITFRemarks = dto.ITFRemarks ?? string.Empty;
                        break;

                    case "Delivery PO Requisition":
                        entity.DeliveryPORequestDate = ToUtc(dto.DeliveryPORequestDate);
                        entity.DeliveryPOIssuanceDate = ToUtc(dto.DeliveryPOIssuanceDate);
                        entity.DeliveryPOTargetDate = ToUtc(dto.DeliveryPOTargetDate);
                        break;

                    case "Test Run PO request":
                        entity.TestRunPORequestDate = ToUtc(dto.TestRunPORequestDate);
                        entity.TestRunPOIssuanceDate = ToUtc(dto.TestRunPOIssuanceDate);
                        break;

                    case "Test Run":
                        entity.TestRunNo = dto.TestRunNo;
                        entity.TestRunActualReceivedDate = ToUtc(dto.TestRunActualReceivedDate);
                        entity.TestRunActualFinishedDate = ToUtc(dto.TestRunActualFinishedDate);
                        entity.TestResult = dto.TestResult;
                        entity.TestRunRemarks = dto.TestRunRemarks ?? string.Empty;
                        break;

                    case "Implementation Date":
                        entity.ImplementationDate = ToUtc(dto.ImplementationDate);
                        break;

                    case "First Delivery Date":
                        entity.FirstDeliveryDate = ToUtc(dto.FirstDeliveryDate);
                        break;

                    default:
                        return BadRequest(new { success = false, message = "Unrecognized process step" });
                }

                entity.CurrentProcess = nextProcess;
                entity.StepOrder = nextMapping?.StepOrder ?? (mapping.StepOrder + 1);
                entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                entity.UpdateDate = DateTime.UtcNow;

                // SaveOther4MProcess
                _dbContext.ActivityCurrentProcesses.Add(new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = nextProcess,
                    UpdateAt = DateTime.UtcNow,
                    ActivityType = "Other4M"   // ← add
                });

                WriteTransactionLog(
                    importData,
                    "Other 4M",
                    dto.ProcessStep,
                    nextProcess,
                    User.Identity?.Name ?? "SYSTEM",
                    GetRemarksForStep(dto));

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new { success = false, message = "Database error", detail = dbEx.InnerException?.Message ?? dbEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private static string GetRemarksForStep(SaveOther4MProcessDto dto) => dto.ProcessStep switch
        {
            "Kataken Evaluation Approval" => dto.KatakenRemarks ?? string.Empty,
            "DE Evaluation" => dto.DERemarks ?? string.Empty,
            "EE Evaluation" => dto.EERemarks ?? string.Empty,
            "QA Evaluation" => dto.QARemarks ?? string.Empty,
            "ITF Process" => dto.ITFRemarks ?? string.Empty,
            "Test Run" => dto.TestRunRemarks ?? string.Empty,
            _ => string.Empty
        };

        // =====================================================================
        // SHARED: Compute LimitDate & RemainingDays for any Renewal step
        // =====================================================================
        private void ComputeLimitAndRemaining<T>(
            List<T> items,
            Func<T, string> getControlNumber,
            Func<T, string> getSection,
            string activityName,
            string process) where T : BasedImportData
        {
            var today = DateTime.UtcNow.Date;

            foreach (var vm in items)
            {
                var controlNumber = getControlNumber(vm);
                var section = getSection(vm);

                // ── TEMP DEBUG — remove after troubleshooting ──
                Console.WriteLine($"[ComputeLimitAndRemaining] controlNumber='{controlNumber}' section='{section}' activityName(process)='{activityName}'");

                // Base date = when this control number entered the current process step
                var acp = _dbContext.ActivityCurrentProcesses
                    .Where(x => x.ControlNumber == controlNumber && x.CurrentProcess.Trim().ToLower() == process.Trim().ToLower())
                    .OrderByDescending(x => x.UpdateAt)
                    .FirstOrDefault();

                // ── TEMP DEBUG ──
                Console.WriteLine($"[ComputeLimitAndRemaining] acp found={acp != null}, acp.UpdateAt={acp?.UpdateAt}");

                var baseDate = (acp?.UpdateAt ?? DateTime.UtcNow).Date;

                // Lead time days from LeadTimes table, matched by Section + Activity only
                // (no per-process granularity in this table — one value covers the whole activity)
                var leadTime = _dbContext.LeadTimes
                    .FirstOrDefault(x =>
                        x.Section.Trim().ToLower() == section.Trim().ToLower() &&
                        x.Activity.Trim().ToLower() == activityName.Trim().ToLower());

                // ── TEMP DEBUG ──
                Console.WriteLine($"[ComputeLimitAndRemaining] leadTime found={leadTime != null}, leadTime.LeadTimeValue={leadTime?.LeadTimeValue}");

                if (leadTime != null)
                {
                    var limitDate = baseDate.AddDays((double)leadTime.LeadTimeValue);
                    vm.LimitDate = limitDate;
                    vm.RemainingDays = (limitDate - today).Days;

                    // ── TEMP DEBUG ──
                    Console.WriteLine($"[ComputeLimitAndRemaining] SET vm.LimitDate={vm.LimitDate}, vm.RemainingDays={vm.RemainingDays}");
                }
                else
                {
                    // ── TEMP DEBUG ──
                    Console.WriteLine($"[ComputeLimitAndRemaining] SKIPPED — no LeadTimes match for section='{section}' activityName='{activityName}'");
                }
            }
        }
    }
}
