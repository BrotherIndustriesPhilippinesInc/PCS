using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Data;
using PartsControlSystem.Models;

namespace PartsControlSystem.Controllers
{
    public class SupplierController : Controller
    {
        private readonly PostgreAppDbContext _dbContext;

        public SupplierController(PostgreAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult SupplierSetting()
        {
            return View();
        }

        [HttpPost("AddSupplier")]
        public IActionResult AddSupplier(Supplier model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    });
                }

                // Handle multiple emails
                var emails = model.Email.Split(';')
                    .Select(e => e.Trim())
                    .Where(e => !string.IsNullOrEmpty(e))
                    .ToList();

                foreach (var email in emails)
                {
                    var supplier = new Supplier
                    {
                        Email = email,
                        SupplierName = model.SupplierName,
                        PartsCategory = model.PartsCategory,      // Add this - it's required!
                        Location = model.Location        // Add this - it's required!
                    };
                    _dbContext.Suppliers.Add(supplier);
                }

                _dbContext.SaveChanges();

                return Json(new
                {
                    success = true,
                    message = $"Added {emails.Count} supplier(s) successfully!"
                });
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessages = new List<string>();
                errorMessages.Add($"Database Error: {dbEx.Message}");

                if (dbEx.InnerException != null)
                {
                    errorMessages.Add($"Details: {dbEx.InnerException.Message}");
                }

                return Json(new
                {
                    success = false,
                    errors = errorMessages
                });
            }
            catch (Exception ex)
            {
                var errorMessages = new List<string>();
                errorMessages.Add(ex.Message);

                if (ex.InnerException != null)
                {
                    errorMessages.Add(ex.InnerException.Message);
                }

                return Json(new
                {
                    success = false,
                    errors = errorMessages
                });
            }
        }

        // ================= GET ALL =================
        [HttpGet("GetSupplier")]
        public IActionResult GetSupplier()
        {
            var suppliers = _dbContext.Suppliers
                .Select(s => new
                {
                    supplierId = s.SupplierId,
                    supplierName = s.SupplierName,
                    partsCategory = s.PartsCategory,
                    location = s.Location,
                    email = s.Email
                })
                .ToList();

            return Json(new { data = suppliers });
        }

        // ================= GET BY ID (EDIT) =================
        [HttpGet]
        public IActionResult GetSupplierById(int id)
        {
            var supplier = _dbContext.Suppliers
                .Where(s => s.SupplierId == id)
                .Select(s => new
                {
                    s.SupplierId,
                    s.SupplierName,
                    s.PartsCategory,
                    s.Location,
                    s.Email
                })
                .FirstOrDefault();

            if (supplier == null)
                return Json(new { success = false, message = "Supplier not found." });

            return Json(supplier);
        }


        // ================= CREATE / UPDATE =================
        [HttpPost]
        public IActionResult Save([FromBody] Supplier model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            if (model.SupplierId == 0)
            {
                _dbContext.Suppliers.Add(model);
            }
            else
            {
                var existing = _dbContext.Suppliers.Find(model.SupplierId);
                if (existing == null)
                    return Json(new { success = false, message = "Supplier not found" });

                existing.SupplierName = model.SupplierName;
                existing.PartsCategory = model.PartsCategory;
                existing.Location = model.Location;
                existing.Email = model.Email;
            }

            _dbContext.SaveChanges();
            return Json(new { success = true });
        }


        // ================= DELETE =================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var supplier = _dbContext.Suppliers.Find(id);
            if (supplier == null)
                return Json(new { success = false, message = "Supplier not found" });

            _dbContext.Suppliers.Remove(supplier);
            _dbContext.SaveChanges();

            return Json(new { success = true });
        }

    }
}
