using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;

namespace PartsControlSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PostgreAppDbContext _postgreAppDbContext;
        private readonly SqlServerAppDbContext _sqlServerAppDbContext;
        private readonly SqlServerAppDbContextCas _sqlServerAppDbContextCas;

        public HomeController(ILogger<HomeController> logger, PostgreAppDbContext postgreAppDbContext, SqlServerAppDbContext sqlServerAppDbContext, SqlServerAppDbContextCas sqlServerAppDbContextCas)
        {
            _logger = logger;
            _sqlServerAppDbContext = sqlServerAppDbContext;
            _postgreAppDbContext = postgreAppDbContext;
            _sqlServerAppDbContextCas = sqlServerAppDbContextCas;
        }


        [Authorize]
        public async Task<IActionResult> Dashboard(
     string? section = null,
     string? startDate = null,
     string? endDate = null)
        {
            var allData = await _postgreAppDbContext.ImportDatas.ToListAsync();
            var allCurrentProcesses = await _postgreAppDbContext.ActivityCurrentProcesses.ToListAsync();
            var leadTimes = await _postgreAppDbContext.LeadTimes.ToListAsync();
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
            // Replace DateCreated with whatever your ImportData date field is called
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

            // ── Per-Activity Stats ───────────────────────────────────────────────────
            var activityStats = new Dictionary<string, (int Done, int Ongoing, int Delay)>
            {
                ["Renewal / Additional Mold"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.RenewalAdditionalMold, completedIqc),
                ["New Tooling / Localization"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.NewToolingLocalization, completedNewTooling),
                ["Supplier Change / Localization"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.SupplierChangeLocalization, completedNewTooling),
                ["Multiple Procurement / Localization"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.MultipleProcurementLocalization, completedNewTooling),
                ["Transfer Tooling"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.TransferTooling, new HashSet<string>()),
                ["Change Material"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.ChangeMaterial, completedChangeMaterial),
                ["New Model"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.NewModel, new HashSet<string>()),
                ["Non-Concurrent"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.NonConcurrent, new HashSet<string>()),
                ["Other 4M"] = GetActivityStats(data, allCurrentProcesses, leadTimes, today, x => x.Other4M, new HashSet<string>()),
            };

            // ── Overall totals (summed from cards — single source of truth) ──────────
            int totalDone = activityStats.Values.Sum(s => s.Done);
            int totalDelay = activityStats.Values.Sum(s => s.Delay);
            int totalOngoing = activityStats.Values.Sum(s => s.Ongoing);

            // ── Delay details per section (scoped to filtered data) ──────────────────
            var delayDetails = new List<(string Section, int DelayItems, int DaysDelay)>();

            var activitySelectors = new List<Func<ImportData, string>>
{
    x => x.RenewalAdditionalMold,
    x => x.NewToolingLocalization,
    x => x.SupplierChangeLocalization,
    x => x.MultipleProcurementLocalization,
    x => x.TransferTooling,
    x => x.ChangeMaterial,
    x => x.NewModel,
    x => x.NonConcurrent,
    x => x.Other4M
};

            foreach (var group in data.GroupBy(x => x.Section ?? "N/A"))
            {
                int sectionDelayItems = 0;
                int sectionMaxDays = 0;

                foreach (var record in group)
                {
                    // Count once per YES activity flag, not once per record
                    foreach (var selector in activitySelectors)
                    {
                        if (!string.Equals(selector(record)?.Trim(), "YES", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var currentProcess = allCurrentProcesses
                            .Where(acp => acp.ControlNumber == record.ControlNo)
                            .OrderByDescending(acp => acp.UpdateAt)
                            .FirstOrDefault();

                        if (currentProcess == null) continue;

                        var leadTime = leadTimes
                            .FirstOrDefault(lt => lt.Activity == currentProcess.CurrentProcess);

                        if (leadTime == null) continue;

                        var deadline = currentProcess.UpdateAt.AddDays((double)leadTime.LeadTimeValue);
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
            ViewBag.SelectedStartDate = startDate;   // raw string — view echoes it into value=""
            ViewBag.SelectedEndDate = endDate;

            return View();
        }

        // ── Helper ───────────────────────────────────────────────────────────────────
        private (int Done, int Ongoing, int Delay) GetActivityStats(
            List<ImportData> data,
            List<ActivityCurrentProcess> allCurrentProcesses,
            List<LeadTime> leadTimes,
            DateTime today,
            Func<ImportData, string> activitySelector,
            HashSet<string> completedControlNos)
        {
            var activityRecords = data
                .Where(x => activitySelector(x)
                    ?.Trim().Equals("YES", StringComparison.OrdinalIgnoreCase) == true)
                .ToList();

            int done = activityRecords.Count(x => completedControlNos.Contains(x.ControlNo));
            int delayed = 0;

            foreach (var record in activityRecords)
            {
                if (completedControlNos.Contains(record.ControlNo)) continue;

                var currentProcess = allCurrentProcesses
                    .Where(acp => acp.ControlNumber == record.ControlNo)
                    .OrderByDescending(acp => acp.UpdateAt)
                    .FirstOrDefault();

                if (currentProcess == null) continue;

                var leadTime = leadTimes
                    .FirstOrDefault(lt => lt.Activity == currentProcess.CurrentProcess);

                if (leadTime == null) continue;

                var deadline = currentProcess.UpdateAt.AddDays((double)leadTime.LeadTimeValue);
                if (deadline < today) delayed++;
            }

            int ongoing = Math.Max(activityRecords.Count - done - delayed, 0);
            return (done, ongoing, delayed);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult IportalConfirmationForm(string? ip = null)
        {
            ViewData["HideFullLayout"] = true; // ✅ hide navbar + sidebar
            // Use the passed IP or fallback to server-side IP
            string userIP = GetClientIp(HttpContext);

            return View(model: userIP);
        }

        public string GetClientIp(HttpContext context)
        {
            // 1️ Check for X-Forwarded-For header (for proxies/load balancers)
            var forwardedHeader = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwardedHeader))
            {
                return forwardedHeader.Split(',')[0].Trim();
            }

            // 2️ Get the remote IP from the connection
            var remoteIp = context.Connection.RemoteIpAddress;

            if (remoteIp != null)
            {
                // Convert IPv6 loopback (::1) to LAN IP for local debugging
                if (remoteIp.IsIPv6LinkLocal || remoteIp.ToString() == "::1" || remoteIp.ToString() == "127.0.0.1")
                {
                    // Try to get actual LAN IP of this machine
                    string lanIp = GetLocalLanIp();
                    return lanIp ?? "127.0.0.1"; // fallback to localhost
                }

                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    return remoteIp.MapToIPv4().ToString();
                }

                return remoteIp.ToString();
            }

            return "IP Not Found";
        }

        // Helper to get LAN IP of current machine
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

            // Force IPv4 format
            if (IPAddress.TryParse(localIP, out var ip) && ip.IsIPv4MappedToIPv6)
                localIP = ip.MapToIPv4().ToString();

            long systemId = 81; // System ID in i-portal

            // Try to get the latest login request for the IP
            var loginEntry = await _sqlServerAppDbContextCas.Tbl_LOGIN_Request
                .Where(x => x.IpAddress == localIP && x.SystemId == systemId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            // If no login request found, redirect to confirmation
            if (loginEntry == null)
            {
                Console.WriteLine("No login request found for IP: " + localIP);
                return RedirectToAction("IportalConfirmationForm", "Home");
            }

            // Lookup user in PostgreSQL
            var user = await _postgreAppDbContext.Users
                .FirstOrDefaultAsync(x => x.EmployeeId == loginEntry.EmployeeId);

            // If user not found, redirect to confirmation
            if (user == null)
            {
                Console.WriteLine("Employee not found for EmployeeId: " + loginEntry.EmployeeId);
                return RedirectToAction("IportalConfirmationForm", "Home");
            }

            // Sign-in with claims
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
    }
}
