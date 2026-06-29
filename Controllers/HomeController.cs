using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
using PartsControlSystem.Helpers;
using PartsControlSystem.Services;

namespace PartsControlSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PostgreAppDbContext _postgreAppDbContext;
        private readonly SqlServerAppDbContext _sqlServerAppDbContext;
        private readonly SqlServerAppDbContextCas _sqlServerAppDbContextCas;
        private readonly MailService _mailService;

        public HomeController(ILogger<HomeController> logger, PostgreAppDbContext postgreAppDbContext, SqlServerAppDbContext sqlServerAppDbContext, SqlServerAppDbContextCas sqlServerAppDbContextCas, MailService mailService)  
        {
            _logger = logger;
            _sqlServerAppDbContext = sqlServerAppDbContext;
            _postgreAppDbContext = postgreAppDbContext;
            _sqlServerAppDbContextCas = sqlServerAppDbContextCas;
            _mailService = mailService;
        }

        [Authorize]
        public async Task<IActionResult> Dashboard(
     string? section = null,
     string? startDate = null,
     string? endDate = null)
        {
            var allData = await _postgreAppDbContext.ImportDatas.ToListAsync();
            var leadTimes = await _postgreAppDbContext.LeadTimes.ToListAsync();
            var other4MMappings = await _postgreAppDbContext.Other4MProcessMappings.ToListAsync();
            var changeMaterialMappings = await _postgreAppDbContext.ChangeMaterialProcessMappings.ToListAsync();
            var newToolingMappings = await _postgreAppDbContext.NewToolingProcessMappings.ToListAsync();

            // ── Per-activity latest process step — single source of truth for status & display ──
            var latestLogsPerActivity = await _postgreAppDbContext.TransactionLogs
                .GroupBy(x => new { x.TransactionNumber, x.Activity })
                .Select(g => g.OrderByDescending(x => x.InputDate).First())
                .ToListAsync();

            var today = DateTime.UtcNow;

            // ── Parse date range ─────────────────────────────────────────────────────
            DateTime? parsedStart = DateTime.TryParse(startDate, out var sd)
                ? sd.Date : null;
            DateTime? parsedEnd = DateTime.TryParse(endDate, out var ed)
                ? ed.Date.AddDays(1) : null;   // +1 so endDate is inclusive

            // ── Section filter ───────────────────────────────────────────────────────
            var data = string.IsNullOrWhiteSpace(section)
                ? allData
                : allData.Where(x => (x.Section ?? "N/A") == section).ToList();

            // ── Date filter ──────────────────────────────────────────────────────────
            if (parsedStart.HasValue)
                data = data.Where(x => x.DateImported >= parsedStart.Value).ToList();
            if (parsedEnd.HasValue)
                data = data.Where(x => x.DateImported < parsedEnd.Value).ToList();

            var filteredControlNos = data.Select(x => x.ControlNo).ToHashSet();

            // ── Completed sets (scoped to filtered data) ─────────────────────────────
            var completedIqc = _postgreAppDbContext.IQCTestRuns
                .Where(t => filteredControlNos.Contains(t.ControlNumber))
                .Select(t => t.ControlNumber)
                .ToHashSet();

            var completedNewTooling = _postgreAppDbContext.NewToolingLocalizationProcesses
                .Where(p => p.CurrentProcess == "Completed" && filteredControlNos.Contains(p.ControlNumber))
                .Select(p => p.ControlNumber)
                .ToHashSet();

            var completedChangeMaterial = _postgreAppDbContext.ChangeMaterialProcesses
                .Where(p => p.ProcessStep == "First Delivery Date" && filteredControlNos.Contains(p.ControlNumber))
                .Select(p => p.ControlNumber)
                .ToHashSet();

            var completedOther4M = _postgreAppDbContext.Other4MProcesses
                .Where(p => p.FirstDeliveryDate != null && filteredControlNos.Contains(p.ControlNumber))
                .Select(p => p.ControlNumber)
                .ToHashSet();

            // ── Per-Activity Stats ───────────────────────────────────────────────────
            var activityStats = new Dictionary<string, (int Done, int Ongoing, int Delay)>
            {
                ["Renewal / Additional Mold"] = GetActivityStats("Renewal / Additional Mold", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.RenewalAdditionalMold, completedIqc),
                ["New Tooling / Localization"] = GetActivityStats("New Tooling / Localization", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.NewToolingLocalization, completedNewTooling),
                ["Supplier Change / Localization"] = GetActivityStats("Supplier Change / Localization", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.SupplierChangeLocalization, completedNewTooling),
                ["Multiple Procurement / Localization"] = GetActivityStats("Multiple Procurement / Localization", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.MultipleProcurementLocalization, completedNewTooling),
                ["Transfer Tooling"] = GetActivityStats("Transfer Tooling", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.TransferTooling, new HashSet<string>()),
                ["Change Material"] = GetActivityStats("Change Material", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.ChangeMaterial, completedChangeMaterial),
                ["New Model"] = GetActivityStats("New Model", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.NewModel, new HashSet<string>()),
                ["Non-Concurrent"] = GetActivityStats("Non-Concurrent", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.NonConcurrent, new HashSet<string>()),
                ["Other 4M"] = GetActivityStats("Other 4M", data, latestLogsPerActivity, leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today, x => x.Other4M, completedOther4M),
            };

            // ── Overall totals (summed from cards — single source of truth) ──────────
            int totalDone = activityStats.Values.Sum(s => s.Done);
            int totalDelay = activityStats.Values.Sum(s => s.Delay);
            int totalOngoing = activityStats.Values.Sum(s => s.Ongoing);

            // ── Delay details per section (scoped to filtered data) ──────────────────
            var delayDetails = new List<(string Section, int DelayItems, int DaysDelay)>();

            var activitySelectors = new List<(string Name, Func<ImportData, string> Selector)>
            {
                ("Renewal / Additional Mold", x => x.RenewalAdditionalMold),
                ("New Tooling / Localization", x => x.NewToolingLocalization),
                ("Supplier Change / Localization", x => x.SupplierChangeLocalization),
                ("Multiple Procurement / Localization", x => x.MultipleProcurementLocalization),
                ("Transfer Tooling", x => x.TransferTooling),
                ("Change Material", x => x.ChangeMaterial),
                ("New Model", x => x.NewModel),
                ("Non-Concurrent", x => x.NonConcurrent),
                ("Other 4M", x => x.Other4M)
            };

            foreach (var group in data.GroupBy(x => x.Section ?? "N/A"))
            {
                int sectionDelayItems = 0;
                int sectionMaxDays = 0;

                foreach (var record in group)
                {
                    foreach (var (activityName, selector) in activitySelectors)
                    {
                        if (!string.Equals(selector(record)?.Trim(), "YES", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var latestLog = ActivityComputationHelper.GetLatestLogForActivity(
                            record.ControlNo, activityName, latestLogsPerActivity);

                        if (latestLog == null) continue;

                        var leadTimeDays = ActivityComputationHelper.ResolveLeadTimeDays(
                            activityName, latestLog.CurrentProcess,
                            leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings);

                        if (leadTimeDays == null || leadTimeDays <= 0) continue;
                        if (latestLog.InputDate == null) continue;

                        var deadline = latestLog.InputDate.Value.AddDays(leadTimeDays.Value);
                        if (deadline < today)
                        {
                            sectionDelayItems++;
                            int daysLate = (int)(today - deadline).TotalDays;
                            if (daysLate > sectionMaxDays) sectionMaxDays = daysLate;
                        }
                    }
                }

                if (sectionDelayItems > 0)
                    delayDetails.Add((Section: group.Key, DelayItems: sectionDelayItems, DaysDelay: sectionMaxDays));
            }

            // ── Section dropdown list (always from allData, unfiltered) ──────────────
            var sections = allData
                .Select(x => x.Section ?? "N/A")
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            ViewBag.TotalDone = totalDone;
            ViewBag.TotalOngoing = totalOngoing;
            ViewBag.TotalDelay = totalDelay;
            ViewBag.DelayDetails = delayDetails;
            ViewBag.ActivityStats = activityStats;
            ViewBag.Sections = sections;
            ViewBag.SelectedSection = section;
            ViewBag.SelectedStartDate = startDate;
            ViewBag.SelectedEndDate = endDate;

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NotifySection(string section)
        {
            if (string.IsNullOrWhiteSpace(section))
                return Json(new { success = false, message = "Section is required." });

            if (!SectionEmails.TryGetValue(section, out var email))
                return Json(new { success = false, message = $"No email configured for section: {section}" });

            // ── Recompute delay details for this section ──────────────────────────
            var allData = await _postgreAppDbContext.ImportDatas.ToListAsync();
            var leadTimes = await _postgreAppDbContext.LeadTimes.ToListAsync();
            var other4MMappings = await _postgreAppDbContext.Other4MProcessMappings.ToListAsync();
            var changeMaterialMappings = await _postgreAppDbContext.ChangeMaterialProcessMappings.ToListAsync();
            var newToolingMappings = await _postgreAppDbContext.NewToolingProcessMappings.ToListAsync();

            var latestLogsPerActivity = await _postgreAppDbContext.TransactionLogs
                .GroupBy(x => new { x.TransactionNumber, x.Activity })
                .Select(g => g.OrderByDescending(x => x.InputDate).First())
                .ToListAsync();

            var today = DateTime.UtcNow;

            var activitySelectors = new List<(string Name, Func<ImportData, string> Selector)>
    {
        ("Renewal / Additional Mold",           x => x.RenewalAdditionalMold),
        ("New Tooling / Localization",           x => x.NewToolingLocalization),
        ("Supplier Change / Localization",       x => x.SupplierChangeLocalization),
        ("Multiple Procurement / Localization",  x => x.MultipleProcurementLocalization),
        ("Transfer Tooling",                     x => x.TransferTooling),
        ("Change Material",                      x => x.ChangeMaterial),
        ("New Model",                            x => x.NewModel),
        ("Non-Concurrent",                       x => x.NonConcurrent),
        ("Other 4M",                             x => x.Other4M),
    };

            var sectionData = allData.Where(x => (x.Section ?? "N/A") == section).ToList();

            // Build delay rows for email table
            var delayRows = new List<(string ControlNo, string Activity, string CurrentProcess, int DaysLate)>();

            foreach (var record in sectionData)
            {
                foreach (var (activityName, selector) in activitySelectors)
                {
                    if (!string.Equals(selector(record)?.Trim(), "YES", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var latestLog = ActivityComputationHelper.GetLatestLogForActivity(
                        record.ControlNo, activityName, latestLogsPerActivity);

                    if (latestLog == null) continue;

                    var leadTimeDays = ActivityComputationHelper.ResolveLeadTimeDays(
                        activityName, latestLog.CurrentProcess,
                        leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings);

                    if (leadTimeDays == null || leadTimeDays <= 0) continue;
                    if (latestLog.InputDate == null) continue;

                    var deadline = latestLog.InputDate.Value.AddDays(leadTimeDays.Value);
                    if (deadline < today)
                    {
                        int daysLate = (int)(today - deadline).TotalDays;
                        delayRows.Add((record.ControlNo, activityName, latestLog.CurrentProcess, daysLate));
                    }
                }
            }

            if (!delayRows.Any())
                return Json(new { success = false, message = $"No delay items found for section {section}." });

            // ── Build email ───────────────────────────────────────────────────────
            var tableRows = string.Join("", delayRows.Select((r, i) => $@"
        <tr>
            <td style='text-align:center;'>{i + 1}</td>
            <td style='text-align:center;'>{r.ControlNo}</td>
            <td>{r.Activity}</td>
            <td>{r.CurrentProcess}</td>
            <td style='text-align:center;color:red;font-weight:bold;'>{r.DaysLate} day(s)</td>
        </tr>"));

            string subject = $"[PCS] Delayed Items Notification – Section {section} ({DateTime.Now:yyyy-MM-dd})";

            string body = $@"
        <p>Dear <strong>{section}</strong> Section,</p>

        <p>Good day!</p>

        <p>
            This is to inform you that the following parts under your section
            have <strong style='color:red;'>exceeded their lead time</strong>
            and are currently marked as <strong>DELAYED</strong>.
        </p>

        <p>Please find the details below:</p>

        <table border='1' cellpadding='8' cellspacing='0'
               style='border-collapse:collapse;width:100%;font-family:Arial;font-size:13px;'>
            <thead>
                <tr style='background-color:#f2f2f2;'>
                    <th>#</th>
                    <th>Control No.</th>
                    <th>Activity</th>
                    <th>Current Process</th>
                    <th>Days Delayed</th>
                </tr>
            </thead>
            <tbody>
                {tableRows}
            </tbody>
        </table>

        <br/>

        <p>
            Kindly coordinate with the responsible PIC to resolve the delays at the soonest possible time.
        </p>

        <p>Thank you.</p>

        <br/>

        <p>
            <strong>Parts Control System Notification</strong><br/>
            <i>This is a system-generated email. Please do not reply.</i>
        </p>";

            await _mailService.SendEmailAsync(email, subject, body);

            return Json(new { success = true, message = $"Notification sent to section {section}." });
        }

        // ── Helper ───────────────────────────────────────────────────────────────────
        private (int Done, int Ongoing, int Delay) GetActivityStats(
       string activityName,
       List<ImportData> data,
       List<TransactionLogs> latestLogsPerActivity,
       List<LeadTime> leadTimes,
       List<NewToolingProcessMapping> newToolingMappings,
       List<ChangeMaterialProcessMapping> changeMaterialMappings,
       List<Other4MProcessMapping> other4MMappings,
       DateTime today,
       Func<ImportData, string> activitySelector,
       HashSet<string> completedControlNos)
        {
            var activityRecords = data
                .Where(x => activitySelector(x)
                    ?.Trim().Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            int done = 0, delayed = 0;

            foreach (var record in activityRecords)
            {
                bool isCompleted = completedControlNos.Contains(record.ControlNo);

                var latestLog = ActivityComputationHelper.GetLatestLogForActivity(
                    record.ControlNo, activityName, latestLogsPerActivity);

                var status = ActivityComputationHelper.ResolveStatus(
                    isCompleted, latestLog, activityName,
                    leadTimes, newToolingMappings, changeMaterialMappings, other4MMappings, today);

                if (status == "Finished") done++;
                else if (status == "Delay") delayed++;
            }

            int ongoing = Math.Max(activityRecords.Count - done - delayed, 0);
            return (done, ongoing, delayed);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult IportalConfirmationForm(string? ip = null)
        {
            ViewData["HideFullLayout"] = true;
            string userIP = GetClientIp(HttpContext);
            return View(model: userIP);
        }

        public string GetClientIp(HttpContext context)
        {
            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            var remoteIp = context.Connection.RemoteIpAddress;

            if (remoteIp != null)
            {
                if (remoteIp.IsIPv6LinkLocal || remoteIp.ToString() == "::1" || remoteIp.ToString() == "127.0.0.1")
                {
                    string lanIp = GetLocalLanIp();
                    return lanIp ?? "127.0.0.1";
                }

                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    return remoteIp.MapToIPv4().ToString();
                }

                return remoteIp.ToString();
            }

            return "IP Not Found";
        }

        private string? GetLocalLanIp()
        {
            foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up)
                    continue;

                var ipProps = ni.GetIPProperties();
                foreach (var ip in ipProps.UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                        !System.Net.IPAddress.IsLoopback(ip.Address))
                    {
                        return ip.Address.ToString();
                    }
                }
            }
            return null;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            string localIP = GetClientIp(HttpContext);

            if (IPAddress.TryParse(localIP, out var ip) && ip.IsIPv4MappedToIPv6)
                localIP = ip.MapToIPv4().ToString();

            long systemId = 81;

            var loginEntry = await _sqlServerAppDbContextCas.Tbl_LOGIN_Request
                .Where(x => x.IpAddress == localIP && x.SystemId == systemId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            if (loginEntry == null)
            {
                Console.WriteLine("No login request found for IP: " + localIP);
                return RedirectToAction("IportalConfirmationForm", "Home");
            }

            var user = await _postgreAppDbContext.Users
                .FirstOrDefaultAsync(x => x.EmployeeId == loginEntry.EmployeeId);

            if (user == null)
            {
                Console.WriteLine("Employee not found for EmployeeId: " + loginEntry.EmployeeId);
                return RedirectToAction("IportalConfirmationForm", "Home");
            }

            var claims = new List<Claim>
            {
                new Claim("EmployeeId", user.EmployeeId ?? ""),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.GivenName, user.FirstName ?? ""),
                new Claim(ClaimTypes.Surname, user.LastName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("Section", user.Section ?? ""),
                new Claim("UserRole", user.Authority ?? ""),
                new Claim("ApproverRole", user.ApproverRole ?? "")
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("MyCookieAuth", principal);

            Console.WriteLine("Login successful for EmployeeId: " + user.EmployeeId);
            return RedirectToAction("Dashboard", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static readonly Dictionary<string, string> SectionEmails = new()
        {
            ["DE"] = "johncarlo.asi@brother-biph.com.ph",
            ["IQC"] = "johncarlo.asi@brother-biph.com.ph",
            ["MP2"] = "johncarlo.asi@brother-biph.com.ph",
            ["QA"] = "johncarlo.asi@brother-biph.com.ph",
            ["SQC"] = "johncarlo.asi@brother-biph.com.ph",
            ["MP1-PUR"] = "johncarlo.asi@brother-biph.com.ph",
            ["MP2-TOOLING"] = "johncarlo.asi@brother-biph.com.ph",
            ["MP2-DOM"] = "johncarlo.asi@brother-biph.com.ph",
            ["MP2-TOOL"] = "johncarlo.asi@brother-biph.com.ph",
            ["PC-DCI"] = "johncarlo.asi@brother-biph.com.ph",
            ["MP2-OVR"] = "johncarlo.asi@brother-biph.com.ph",
            ["MP1"] = "johncarlo.asi@brother-biph.com.ph",
        };
    }
}