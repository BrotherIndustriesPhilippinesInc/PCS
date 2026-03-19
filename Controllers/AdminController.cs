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

        //[HttpPost]
        //public IActionResult CreateAccount([FromBody] User user)
        //{
        //    if (user == null)
        //        return Json(new { success = false, message = "Invalid user data." });

        //    try
        //    {
        //        // Check duplicate Employee ID
        //        bool exists = _dbContext.Users.Any(u => u.EmployeeId == user.EmployeeId);

        //        if (exists)
        //            return Json(new { success = false, message = "User already exists." });

        //        user.Created = DateTime.UtcNow;
        //        _dbContext.Users.Add(user);
        //        _dbContext.SaveChanges();  

        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = ex.InnerException?.Message ?? ex.Message;
        //        return Json(new { success = false, message = errorMessage });
        //    }
        //}

       

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


        //[HttpPost("AddSupplier")]
        //public IActionResult AddSupplier(Supplier model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Json(new
        //            {
        //                success = false,
        //                errors = ModelState.Values
        //                    .SelectMany(v => v.Errors)
        //                    .Select(e => e.ErrorMessage)
        //                    .ToList()
        //            });
        //        }

        //        // Handle multiple emails
        //        var emails = model.Email.Split(';')
        //            .Select(e => e.Trim())
        //            .Where(e => !string.IsNullOrEmpty(e))
        //            .ToList();

        //        foreach (var email in emails)
        //        {
        //            var supplier = new Supplier
        //            {
        //                Email = email,
        //                SupplierName = model.SupplierName,
        //                PartsCategory = model.PartsCategory,      // Add this - it's required!
        //                Location = model.Location        // Add this - it's required!
        //            };
        //            _dbContext.Suppliers.Add(supplier);
        //        }

        //        _dbContext.SaveChanges();

        //        return Json(new
        //        {
        //            success = true,
        //            message = $"Added {emails.Count} supplier(s) successfully!"
        //        });
        //    }
        //    catch (DbUpdateException dbEx)
        //    {
        //        var errorMessages = new List<string>();
        //        errorMessages.Add($"Database Error: {dbEx.Message}");

        //        if (dbEx.InnerException != null)
        //        {
        //            errorMessages.Add($"Details: {dbEx.InnerException.Message}");
        //        }

        //        return Json(new
        //        {
        //            success = false,
        //            errors = errorMessages
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        var errorMessages = new List<string>();
        //        errorMessages.Add(ex.Message);

        //        if (ex.InnerException != null)
        //        {
        //            errorMessages.Add(ex.InnerException.Message);
        //        }

        //        return Json(new
        //        {
        //            success = false,
        //            errors = errorMessages
        //        });
        //    }
        //}
    }
}
