using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.DTO;
using PartsControlSystem.Models;
using PartsControlSystem.Services;
using PartsControlSystem.ViewModels;
using static Syncfusion.XlsIO.Parser.Biff_Records.Charts.ChartAlrunsRecord;

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

        //public IActionResult UpdateActivity()
        //{
        //    return View();
        //}

        [HttpGet("ActivityMonitoring")]
        public async Task<IActionResult> ActivityMonitoring()
        {
            var data = await _dbContext.ViewActivityMonitoring
                .AsNoTracking()
                .OrderBy(x => x.ControlNumber)
                .ToListAsync();

            return View(data);
        }

        [HttpGet("UpdateActivity")]
        public async Task<IActionResult> UpdateActivity()
        {
            // Load all data
            var data = await _dbContext.ImportDatas.ToListAsync();
            int totalEntries = data.Count;

            // Get all unique sections
            var allSections = data
                .Select(x => x.Section ?? "N/A")   // handle null sections
                .Distinct()
                .OrderBy(s => s)                   // optional: sort alphabetically
                .ToList();

            // 3️⃣ Helper: Count "YES" per section for a given column
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

            // Prepare activity cards
            var activityCards = new List<ActivityCardViewModel>
            {
                new ActivityCardViewModel
                {
                    ActivityName = "Renewal / Additional Mold",
                    SectionCounts = GetSectionCounts(x => x.RenewalAdditionalMold)
                },
                new ActivityCardViewModel
                {
                    ActivityName = "New Tooling / Localization",
                    SectionCounts = GetSectionCounts(x => x.NewToolingLocalization)
                },
                new ActivityCardViewModel
                {
                    ActivityName = "Transfer Tooling",
                    SectionCounts = GetSectionCounts(x => x.TransferTooling)
                },
                new ActivityCardViewModel
                {
                    ActivityName = "Change Material",
                    SectionCounts = GetSectionCounts(x => x.ChangeMaterial)
                },
                new ActivityCardViewModel
                {
                    ActivityName = "New Model",
                    SectionCounts = GetSectionCounts(x => x.NewModel)
                },
                new ActivityCardViewModel
                {
                    ActivityName = "Non-Concurrent",
                    SectionCounts = GetSectionCounts(x => x.NonConcurrent)
                },
                new ActivityCardViewModel
                {
                    ActivityName = "Supplier Change / Localization",
                    SectionCounts = GetSectionCounts(x => x.SupplierChangeLocalization)
                },
                new ActivityCardViewModel
                {
                    ActivityName = "Other 4M",
                    SectionCounts = GetSectionCounts(x => x.Other4M)
                }
                // add more activities if needed
            };

            // Pass total entries to the view 
            ViewBag.TotalEntries = totalEntries;

            return View(activityCards);
        }

        [HttpGet("ActivityDetails")]
        public IActionResult ActivityDetails(string activityName)
        {
            IQueryable<ImportData> query = _dbContext.ImportDatas;

            query = activityName switch
            {
                "Renewal / Additional Mold" =>
                    query.Where(x => x.RenewalAdditionalMold == "YES"),

                "New Tooling / Localization" =>
                    query.Where(x => x.NewToolingLocalization == "YES"),

                "Transfer Tooling" =>
                    query.Where(x => x.TransferTooling == "YES"),

                "Change Material" =>
                    query.Where(x => x.ChangeMaterial == "YES"),

                "New Model" =>
                    query.Where(x => x.NewModel == "YES"),

                "Non-Concurrent" =>
                    query.Where(x => x.NonConcurrent == "YES"),

                "Supplier Change / Localization" =>
                    query.Where(x => x.SupplierChangeLocalization == "YES"),

                "Other 4M" =>
                    query.Where(x => x.Other4M == "YES"),

                _ => query.Where(x => false) // safety
            };

            var importDataList = query
                .OrderBy(x => x.Section)
                .ThenBy(x => x.ControlNo)
                .ToList();

            // create the ActivityCardViewModel
            var activityCard = new ActivityCardViewModel
            {
                ActivityName = activityName
            };

            // create the combined viewmodel
            var model = new ActivityImportViewModel
            {
                ActivityCard = activityCard,
                ImportDataList = importDataList
            };

            return View(model);
        }


        [HttpGet("GetActivitiesBySection")]
        public IActionResult GetActivitiesBySection(string section)
        {
            // var userSection = User.FindFirst("Section")?.Value;
            // if (section != userSection) return Unauthorized();

            var activities = _dbContext.LeadTimes
                .Where(x => x.Section == section) // use querystring for testing
                .Select(x => x.Activity)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            return Json(activities);
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
            {
                return Json(new
                {
                    success = true,
                    columns = new List<string>(),
                    data = importData
                });
            }

            var allowedColumns = matrixRow.GetType()
                .GetProperties()
                .Where(p =>
                    p.PropertyType == typeof(bool) &&
                    (bool)p.GetValue(matrixRow) &&
                    p.Name != "IsActive" &&
                    p.Name != "IsDeleted"
                )
                .Select(p => p.Name)
                .ToList();

            return Json(new
            {
                success = true,
                columns = allowedColumns,
                data = importData
            });
        }


        [HttpGet("GetPartial")]
        public async Task<IActionResult> GetPartial(
        string section,
        string activity,
        string process)
        {
            IQueryable<ImportData> query = _dbContext.ImportDatas;

            //ACTIVITY → DATA FILTER
            query = activity switch
            {
                "Renewal / Additional Mold" =>
                    query.Where(x => x.RenewalAdditionalMold == "YES"),

                "New Tooling / Localization" =>
                    query.Where(x => x.NewToolingLocalization == "YES"),

                "Transfer Tooling" =>
                    query.Where(x => x.TransferTooling == "YES"),

                "Change Material" =>
                    query.Where(x => x.ChangeMaterial == "YES"),

                "New Model" =>
                    query.Where(x => x.NewModel == "YES"),

                "Non-Concurrent" =>
                    query.Where(x => x.NonConcurrent == "YES"),

                "Supplier Change / Localization" =>
                    query.Where(x => x.SupplierChangeLocalization == "YES"),

                "Other 4M" =>
                    query.Where(x => x.Other4M == "YES"),

                _ => query.Where(x => false)
            };

            //PROCESS → PARTIAL VIEW ONLY
            return process switch
            {
                "Tooling Quotation Request~Approval" => await HandleToolingQuotationRequestApproval(query, process),

                "Tooling Request-Order" => await HandleToolingRequestOrder(query, process),

                "Tooling PO Issuance" => await HandleToolingPoIssuance(query, process),

                "DFM/QCD Approval" => await HandleDFMQCDApproval(query, process),

                "Tooling Fabrication" => await HandleToolingFabrication(query, process),

                "Tooling Transfer (Arrival in PH)" => await HandleToolingTransfer(query, process),

                "Kataken Submission (Local Trial)" => await HandleKatakenSubmission(query, process),

                "Kataken Finish (Local Trial)" => await HandleKatakenFinish(query, process),

                "DE Evaluation" => await HandleEvaluation(query, process),

                "QA Special Evaluation" => await HandleSpecialEvaluation(query, process),

                "Test Run" => await HandleTestRun(query, process),

                _ => Content("<div class='text-muted'>No process partial defined</div>")
            };
        }

        private async Task<IActionResult> HandleToolingQuotationRequestApproval(
        IQueryable<ImportData> query,
        string process)
        {
            var list = await query
                .Where(importData =>
                    !_dbContext.ToolingQuotationRequestApproval
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!list.Any())
            {
                return Content(@"
                    <div class='alert alert-warning text-center mt-3'>
                        <i class='fa-solid fa-triangle-exclamation me-2'></i>
                        No imported data for this process yet.
                    </div>
                ");
            }

            return PartialView(
                "Partials/MP2/_MP2ToolingQuotationRequestApproval",
                list.Select(a => _updateActivityMapperService.MapQuotationRequest(a))
            );
        }

        private async Task<IActionResult> HandleToolingRequestOrder(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                       .Where(importData =>
                           !_dbContext.MP2ToolingRequestOrder
                               .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                           &&
                           _dbContext.ActivityCurrentProcesses
                               .Any(acp =>
                                   acp.ControlNumber == importData.ControlNo &&
                                   acp.CurrentProcess == process
                               )
                       )
                       .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/MP2/_MP2ToolingRequestOrder",
                requestOrderList.Select(activity => _updateActivityMapperService.MapRequestOrder(activity))
            );
        }

        private async Task<IActionResult> HandleToolingPoIssuance(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                       .Where(importData =>
                           !_dbContext.MP2ToolingPoIssuance
                               .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                           &&
                           _dbContext.ActivityCurrentProcesses
                               .Any(acp =>
                                   acp.ControlNumber == importData.ControlNo &&
                                   acp.CurrentProcess == process
                               )
                       )
                       .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/MP2/_MP2ToolingPOIssuance",
                requestOrderList.Select(activity => _updateActivityMapperService.MapPoIssuance(activity))
            );
        }

        private async Task<IActionResult> HandleDFMQCDApproval(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                .Where(importData =>
                    !_dbContext.SQCDFMQCDApprovals
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/SQC/_SQCDFM_QCDApproval",
                requestOrderList.Select(activity => _updateActivityMapperService.MapDfmQcdApproval(activity))
            );
        }

        private async Task<IActionResult> HandleToolingFabrication(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                .Where(importData =>
                    !_dbContext.MP2ToolingFabrications
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/MP2/_MP2ToolingFabrication",
                requestOrderList.Select(activity => _updateActivityMapperService.MapToolingFabrication(activity))
            );
        }

        private async Task<IActionResult> HandleToolingTransfer(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                .Where(importData =>
                    !_dbContext.MP2ToolingTransfers
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/MP2/_MP2ToolingTransfer",
                requestOrderList.Select(activity => _updateActivityMapperService.MapToolingTransfer(activity))
            );
        }

        private async Task<IActionResult> HandleKatakenSubmission(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                .Where(importData =>
                    !_dbContext.IQCKatakenSubmissions
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/IQC/_IQCKatakenSubmission",
                requestOrderList.Select(activity => _updateActivityMapperService.MapKatakenSubmission(activity))
            );
        }

        private async Task<IActionResult> HandleKatakenFinish(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                .Where(importData =>
                    !_dbContext.IQCKatakenFinish
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/IQC/_IQCKatakenFinish",
                requestOrderList.Select(activity => _updateActivityMapperService.MapKatakenFinish(activity))
            );
        }

        private async Task<IActionResult> HandleEvaluation(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                .Where(importData =>
                    !_dbContext.DEEvaluation
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/DE/_DEEvaluation",
                requestOrderList.Select(activity => _updateActivityMapperService.MapEvaluation(activity))
            );
        }

        private async Task<IActionResult> HandleSpecialEvaluation(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                .Where(importData =>
                    !_dbContext.QASpecialEvaluations
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/QA/_QASpecialEvaluation",
                requestOrderList.Select(activity => _updateActivityMapperService.MapSpecialEvaluation(activity))
            );
        }

        private async Task<IActionResult> HandleTestRun(
        IQueryable<ImportData> query,
        string process)
        {
            var requestOrderList = await query
                .Where(importData =>
                    !_dbContext.IQCTestRuns
                        .Any(mp2 => mp2.ControlNumber == importData.ControlNo)
                    &&
                    _dbContext.ActivityCurrentProcesses
                        .Any(acp =>
                            acp.ControlNumber == importData.ControlNo &&
                            acp.CurrentProcess == process
                        )
                )
                .ToListAsync();

            if (!requestOrderList.Any())
            {
                return Content(@"
                        <div class='alert alert-warning text-center mt-3'>
                            <i class='fa-solid fa-triangle-exclamation me-2'></i>
                            No imported data for this process yet.
                        </div>
                    ");
            }

            return PartialView(
                "Partials/IQC/_IQCTestRun",
                requestOrderList.Select(activity => _updateActivityMapperService.MapTestRun(activity))
            );
        }

        [HttpPost("SaveMP2ToolingQuotationRequestApprovalUpdate")]
        public IActionResult SaveMP2ToolingQuotationRequestApprovalUpdate([FromBody] SaveMP2ToolingQuotationRequestApprovalVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.ToolingQuotationRequestApproval
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new MP2_ToolingQuotationRequestApproval
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        TargetDate = ToUtc(dto.TargetDate),
                        ActualDate = ToUtc(dto.ActualDate),
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        Process = "Tooling Quotation Request~Approval",
                        CurrentProcess = "Tooling Request-Order"
                    };

                    _dbContext.ToolingQuotationRequestApproval.Add(entity);
                } 
                {
                    entity.TargetDate = ToUtc(dto.TargetDate);
                    entity.ActualDate = ToUtc(dto.ActualDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Tooling Request-Order";
                }

                string currentProcessValue = "Tooling Request-Order";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpPost("SaveMP2ActivityUpdate")]
        public IActionResult SaveMP2ActivityUpdate([FromBody] SaveMP2ActivityUpdateDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.MP2ToolingRequestOrder
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new MP2_ToolingRequestOrder
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        TRFNo = dto.TRFNo ?? "N/A",
                        TargetDate = ToUtc(dto.TargetDate),
                        ActualDate = ToUtc(dto.ActualDate),
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
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
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                    entity.CurrentProcess = "Tooling PO Issuance";
                }

                string currentProcessValue = "Tooling PO Issuance";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveMP2ToolingPoIssuance")]
        public IActionResult SaveMP2ToolingPoIssuance([FromBody] SaveMP2ToolingPoIssuanceVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.MP2ToolingPoIssuance
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new MP2_ToolingPoIssuance
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        TargetIssueDate = ToUtc(dto.TargetIssueDate),
                        ActualIssueDate = ToUtc(dto.ActualIssueDate),
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "DFM/QCD Approval"
                    };

                    _dbContext.MP2ToolingPoIssuance.Add(entity);
                }
                {
                    entity.TargetIssueDate = ToUtc(dto.TargetIssueDate);
                    entity.ActualIssueDate = ToUtc(dto.ActualIssueDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "DFM/QCD Approval";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveSQCDfmQcdApproval")]
        public IActionResult SaveSQCDfmQcdApproval([FromBody] SaveSQCDfmQcdApprovalVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.SQCDFMQCDApprovals
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new SQC_DFMQCDApproval
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        NeedNoNeedQcdMtg = dto.NeedNoNeedQcdMtg,
                        ApprovalLeadTime = dto.ApprovalLeadTime,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Tooling Fabrication"
                    };

                    _dbContext.SQCDFMQCDApprovals.Add(entity);
                }
                {

                    entity.NeedNoNeedQcdMtg = dto.NeedNoNeedQcdMtg;
                    entity.ApprovalLeadTime = dto.ApprovalLeadTime;
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "Tooling Fabrication";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveMP2ToolingFabrication")]
        public IActionResult SaveMP2ToolingFabrication([FromBody] SaveToolingFabricationVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.MP2ToolingFabrications
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new MP2_ToolingFabrication
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        FabricationLeadTime = dto.FabricationLeadTime,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Tooling Transfer (Arrival in PH)"
                    };

                    _dbContext.MP2ToolingFabrications.Add(entity);
                }
                {
                    entity.FabricationLeadTime = dto.FabricationLeadTime;
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "Tooling Transfer (Arrival in PH)";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveMP2ToolingTransfer")]
        public IActionResult SaveMP2ToolingTransfer([FromBody] SaveToolingTransferVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.MP2ToolingTransfers
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new MP2_ToolingTransfer
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        TransferLeadTime = dto.TransferLeadTime,
                        TargetArrivalDate = ToUtc(dto.TargetArrivalDate),
                        ActualArrivalDate = ToUtc(dto.ActualArrivalDate),
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Kataken Submission (Local Trial)"
                    };

                    _dbContext.MP2ToolingTransfers.Add(entity);
                }
                {
                    entity.TransferLeadTime = dto.TransferLeadTime;
                    entity.TargetArrivalDate = ToUtc(dto.TargetArrivalDate);
                    entity.ActualArrivalDate = ToUtc(dto.ActualArrivalDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "Kataken Submission (Local Trial)";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveIQCKatakenSubmission")]
        public IActionResult SaveIQCKatakenSubmission([FromBody] SaveKatakenSubmissionVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.IQCKatakenSubmissions
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new IQC_KatakenSubmission
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        ActualSubmissionDate = ToUtc(dto.ActualSubmissionDate),
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Kataken Finish (Local Trial)"
                    };

                    _dbContext.IQCKatakenSubmissions.Add(entity);
                }
                {
                    entity.ActualSubmissionDate = ToUtc(dto.ActualSubmissionDate);
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "Kataken Finish (Local Trial)";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveIQCKatakenFinish")]
        public IActionResult SaveIQCKatakenFinish([FromBody] SaveKatakenFinishVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.IQCKatakenFinish
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new IQC_KatakenFinish
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Result = dto.Result,
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "DE Evaluation"
                    };

                    _dbContext.IQCKatakenFinish.Add(entity);
                }
                {
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Result = dto.Result;
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "DE Evaluation";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveDEEvaluation")]
        public IActionResult SaveDEEvaluation([FromBody] SaveEvaluationVMDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.DEEvaluation
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new DE_Evaluation
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        NeedNoNeed = dto.NeedNoNeed,
                        LeadTime = dto.LeadTime,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Result = dto.Result,
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "QA Special Evaluation"
                    };

                    _dbContext.DEEvaluation.Add(entity);
                }
                {
                    entity.NeedNoNeed = dto.NeedNoNeed;
                    entity.LeadTime = dto.LeadTime;
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Result = dto.Result;
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "QA Special Evaluation";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveQASpecialEvaluation")]
        public IActionResult SaveQASpecialEvaluation([FromBody] SaveSpecialEvaluationDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.QASpecialEvaluations
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new QA_SpecialEvaluation
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        NeedNoNeed = dto.NeedNoNeed,
                        LeadTime = dto.LeadTime,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        Result = dto.Result,
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "Test Run"
                    };

                    _dbContext.QASpecialEvaluations.Add(entity);
                }
                {
                    entity.NeedNoNeed = dto.NeedNoNeed;
                    entity.LeadTime = dto.LeadTime;
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.Result = dto.Result;
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "Test Run";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("SaveIQCTestRun")]
        public IActionResult SaveIQCTestRun([FromBody] SaveTestRunDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ControlNumber))
                return BadRequest(new { success = false, message = "Invalid data" });

            try
            {
                var entity = _dbContext.IQCTestRuns
                    .FirstOrDefault(x => x.ControlNumber == dto.ControlNumber);

                if (entity == null)
                {
                    var importData = _dbContext.ImportDatas
                        .FirstOrDefault(x => x.ControlNo == dto.ControlNumber);

                    if (importData == null)
                        return NotFound(new { success = false, message = "Control number not found" });

                    string activity = DetermineActivity(importData);

                    entity = new IQC_TestRun
                    {
                        ControlNumber = dto.ControlNumber,
                        Section = importData.Section,
                        Activity = activity,
                        ActualFinishDate = ToUtc(dto.ActualFinishDate),
                        ResultEmailDatetoSupplier = ToUtc(dto.ResultEmailDatetoSupplier),
                        ResultPassedFailed = dto.ResultPassedFailed,
                        Remarks = dto.Remarks ?? string.Empty,  // Use empty string instead of null
                        InputBy = User.Identity?.Name ?? "SYSTEM",
                        CreateDate = DateTime.UtcNow,
                        CurrentProcess = "MP2-PDC"
                    };

                    _dbContext.IQCTestRuns.Add(entity);
                }
                {
                    entity.ActualFinishDate = ToUtc(dto.ActualFinishDate);
                    entity.ResultEmailDatetoSupplier = ToUtc(dto.ResultEmailDatetoSupplier);
                    entity.ResultPassedFailed = dto.ResultPassedFailed;
                    entity.Remarks = dto.Remarks ?? string.Empty;  // Use empty string instead of null
                    entity.InputBy = User.Identity?.Name ?? "SYSTEM";
                }

                string currentProcessValue = "MP2-PDC";
                entity.CurrentProcess = currentProcessValue;

                var activityProcess = new ActivityCurrentProcess
                {
                    ControlNumber = dto.ControlNumber,
                    CurrentProcess = currentProcessValue,
                    UpdateAt = DateTime.UtcNow
                };

                _dbContext.ActivityCurrentProcesses.Add(activityProcess);

                _dbContext.SaveChanges();
                return Ok(new { success = true });
            }
            catch (DbUpdateException dbEx)
            {
                var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error",
                    detail = innerMessage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private DateTime? ToUtc(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
        }

        private string DetermineActivity(ImportData data)
        {
            if (data.RenewalAdditionalMold == "YES")
                return "Renewal / Additional Mold";
            if (data.NewToolingLocalization == "YES")
                return "New Tooling / Localization";
            if (data.TransferTooling == "YES")
                return "Transfer Tooling";
            if (data.ChangeMaterial == "YES")
                return "Change Material";
            if (data.NewModel == "YES")
                return "New Model";
            if (data.NonConcurrent == "YES")
                return "Non-Concurrent";
            if (data.SupplierChangeLocalization == "YES")
                return "Supplier Change / Localization";
            if (data.Other4M == "YES")
                return "Other 4M";

            return "Unknown";
        }

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
    }
}
