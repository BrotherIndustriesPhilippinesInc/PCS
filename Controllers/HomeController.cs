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
        public IActionResult Dashboard()
        {
            return View();
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
