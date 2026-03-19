using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsControlSystem.Data;

namespace PartsControlSystem.Controllers
{
    [Route("Employee")]
    public class EmployeeController : Controller
    {
        private readonly SqlServerAppDbContext _dbContext;

        public EmployeeController(SqlServerAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        [HttpGet("GetDetailsById")]
        public IActionResult GetDetailsById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest(new { success = false, message = "Employee ID is required." });

                var user = _dbContext.T_Employee_List
                    .FirstOrDefault(u => u.EmployeeId.Trim().ToUpper() == id.Trim().ToUpper());

                if (user == null)
                    return NotFound(new { success = false, message = "Employee not found." });

                return Json(new
                {
                    success = true,
                    adid = user.ADID,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    section = user.Section,
                    department = user.Department,
                    position = user.Position
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal Server Error",
                    detail = ex.Message
                });
            }
        }

    }
}
