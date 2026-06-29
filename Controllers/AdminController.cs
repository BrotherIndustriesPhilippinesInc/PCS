using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;

namespace PartsControlSystem.Controllers
{

    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly PostgreAppDbContext _dbContext;
        private readonly SqlServerAppDbContextCas _sqlServerAppDbContextCas;

        public AdminController(PostgreAppDbContext dbContext, SqlServerAppDbContextCas sqlServerAppDbContextCas)
        {
            _dbContext = dbContext;
            _sqlServerAppDbContextCas = sqlServerAppDbContextCas;
        }

        public IActionResult AdminAccess()
        {
            // Fetch all users from database
            var users = _dbContext.Users.ToList(); // never null
            return View(users);
        }


        [HttpPost]
        public IActionResult CreateAccount([FromBody] User user)
        {
            if (user == null)
                return Json(new { success = false, message = "Invalid user data." });

            try
            {
                // 1️⃣ Check duplicate user in PostgreSQL
                bool exists = _dbContext.Users.Any(u => u.EmployeeId == user.EmployeeId);
                if (exists)
                    return Json(new { success = false, message = "User already exists." });

                // 2️⃣ Save User (PostgreSQL)
                user.Created = DateTime.UtcNow;
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();


                // 4️⃣ Insert Approver (SQL Server)
                var approver = new CasSystemApproverList
                {
                    SystemID = "81",
                    SystemName = "Parts Control System",
                    ApproverNumber = 0,

                    FullName = $"{user.FirstName} {user.LastName}",
                    EmailAddress = user.Email,
                    Section = user.Section,
                    //Position = user.Position,

                    ADID = user.ADID,
                    EmployeeNumber = user.EmployeeId
                };

                _sqlServerAppDbContextCas.Tbl_System_Approver_list.Add(approver);
                _sqlServerAppDbContextCas.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }



        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _dbContext.Users
                .Select(u => new
                {
                    u.EmployeeId,
                    u.ADID,
                    FullName = u.FirstName + " " + u.LastName,
                    u.Email,
                    u.Section,
                    u.Department,
                    u.Authority,
                    Created = u.Created.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") // convert from UTC to local
                })
                .ToList();

            return Json(new { data = users });
        }

        [HttpPost("DeleteUsers")]
        public IActionResult DeleteUsers([FromForm] List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return Json(new { success = false, message = "No users selected." });

            var users = _dbContext.Users.Where(u => userIds.Contains(u.EmployeeId)).ToList();

            if (!users.Any())
                return Json(new { success = false, message = "Selected users not found." });

            _dbContext.Users.RemoveRange(users);
            _dbContext.SaveChanges();

            return Json(new { success = true, successMessage = $"{users.Count} user(s) deleted successfully!" });
        }
    }
}
