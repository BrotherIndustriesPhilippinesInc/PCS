using System.Net.Mail;
using System.Security.Claims;
using iText.Commons.Actions.Contexts;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using PartsControlSystem.Data;
using PartsControlSystem.Models;
using PartsControlSystem.Services;
using PartsControlSystem.ViewModels;
using Syncfusion.XlsIO;
using Syncfusion.Pdf;
using PdfDocument = Syncfusion.Pdf.PdfDocument;
using Syncfusion.XlsIORenderer;
using ClosedXML.Excel;
using System.Text;
using System.Diagnostics;



namespace PartsControlSystem.Controllers
{
    public class _4mFormController : Controller
    {
        private readonly PostgreAppDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly MailService _mailService;
        private readonly IConfiguration _config;

        public _4mFormController(PostgreAppDbContext dbContext, IWebHostEnvironment env, MailService mailService, IConfiguration config)
        {
            _dbContext = dbContext;
            _env = env;
            _mailService = mailService;
            _config = config;
        }

        [Authorize] // <-- Require login
        public IActionResult FourMForm()
        {
            // Get logged-in user's approver role
            string approverRole = User.Claims.FirstOrDefault(c => c.Type == "ApproverRole")?.Value;

            // Fetch all forms
            var allForms = _dbContext._4mForms.ToList();

            // Map UserRole to the corresponding Status in the forms
            string statusForApproval = approverRole switch
            {
                "ROHS PIC" => "FOR ROHS PIC APPROVAL",
                "PSUG PIC" => "FOR PSUG PIC APPROVAL",
                "SUPERVISOR" => "FOR SUPERVISOR APPROVAL",
                "MANAGER" => "FOR MANAGER APPROVAL",
                _ => "" // unknown role → no approvals
            };

            // Filter forms by mapped status
            var approvalForms = allForms
                .Where(f => f.Status == statusForApproval)
                .DistinctBy(f => f.ControlNumber)
                .ToList();

            var vm = new FourMFormViewModel
            {
                DataEntry = new PartsControlSystem.Models._4mForm(),
                FormUpdateList = allForms,
                ApprovalList = approvalForms, // filtered by role
                SendingList = allForms.Where(f => f.Status == "APPROVED" && f.IsEmailSent == false).ToList()
            };

            return View(vm);
        }

        //Download template for batch import
        public IActionResult Download4MTemplate()
        {
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "templates",
                "4MForm_Batch Data Entry Template.xlsx");

            return PhysicalFile(
                path,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "4MForm_Batch Data Entry Template.xlsx");
        }


        [HttpGet]
        public async Task<IActionResult> LoadFormUpdatePartial(string keyword = "")
        {
            var query = _dbContext._4mForms.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(f =>
                    f.ControlNumber.ToLower().Contains(keyword) ||
                    f.CompanyName.ToLower().Contains(keyword) ||
                    f.SupplierPIC.ToLower().Contains(keyword) ||
                    f.TypeOfChange.ToLower().Contains(keyword) ||
                    f.PartCode.ToLower().Contains(keyword) ||
                    f.PartName.ToLower().Contains(keyword) ||
                    f.ChangeReason.ToLower().Contains(keyword) ||
                    f.PQCPIC.ToLower().Contains(keyword)
                );
            }

            var list = await query
                .Where(f => f.HasReply == false && (f.Status == "" || f.Status == "-"))
                .OrderBy(f => f.SupplierSubmissionDate)
                .ToListAsync();

            Console.WriteLine("Keyword: " + keyword);

            return PartialView("_FormUpdate", list);

        }

        [HttpGet]
        public IActionResult LoadFormApprovalPartial()
        {
            // Get logged-in user's approver role
            string approverRole = User.Claims
                .FirstOrDefault(c => c.Type == "ApproverRole")?.Value;

            if (string.IsNullOrEmpty(approverRole))
            {
                return PartialView("_FormApproval", new List<_4mForm>());
            }

            // Map role → status
            string statusForApproval = approverRole switch
            {
                "ROHS PIC" => "FOR ROHS PIC APPROVAL",
                "PSUG PIC" => "FOR PSUG PIC APPROVAL",
                "SUPERVISOR" => "FOR SUPERVISOR APPROVAL",
                "MANAGER" => "FOR MANAGER APPROVAL",
                _ => null
            };

            if (string.IsNullOrEmpty(statusForApproval))
            {
                return PartialView("_FormApproval", new List<_4mForm>());
            }

            // Fetch ONLY needed forms (avoid ToList() early)
            var approvalForms = _dbContext._4mForms
                .Where(f => f.Status == statusForApproval)
                .OrderByDescending(f => f.Id)
                .ToList();

            return PartialView("_FormApproval", approvalForms);
        }

        [HttpGet]
        public IActionResult LoadFormSendingPartial()
        {
            // Fetch forms with Status = "APPROVED" only
            var sendingForms = _dbContext._4mForms
                .Where(f => f.Status == "APPROVED" && f.IsEmailSent == false)
                .OrderByDescending(f => f.Id)
                .ToList();

            // Return the same partial view (you can create a separate one if needed)
            return PartialView("_FormSending", sendingForms);
        }


        private async Task<string> GenerateControlNumber()
        {
            var now = DateTime.Now;
            var yearMonth = now.ToString("yyMM"); // 2601

            var lastRecord = await _dbContext._4mForms
                .Where(x => x.ControlNumber.StartsWith($"BIPH4M-{yearMonth}"))
                .OrderByDescending(x => x.ControlNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;
            if (lastRecord != null)
            {
                var lastSeq = lastRecord.ControlNumber.Split('-').Last();
                nextSequence = int.Parse(lastSeq) + 1;
            }

            return $"BIPH4M-{yearMonth}-{nextSequence:D3}";
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplierCompanies()
        {
            try
            {
                var companies = await _dbContext.Suppliers
                    .Select(s => s.SupplierName)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToListAsync();

                return Json(companies);
            }
            catch (Exception ex)
            {
                return Json(new List<string>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([FromForm] _4mForm model)
        {
            try
            {
                // Skip validation for generated fields
                ModelState.Remove(nameof(_4mForm.ControlNumber));
                ModelState.Remove(nameof(_4mForm.AttachmentPath));
                ModelState.Remove(nameof(_4mForm.Status));
                ModelState.Remove(nameof(_4mForm.ApprovedDate));
                ModelState.Remove(nameof(_4mForm.PdfAttachmentPath));

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToArray();
                    return Json(new { success = false, message = string.Join("; ", errors) });
                }

                // Generate Control Number  
                model.ControlNumber = await GenerateControlNumber();

                // Convert date to UTC for PostgreSQL
                model.SupplierSubmissionDate = DateTime.SpecifyKind(model.SupplierSubmissionDate, DateTimeKind.Utc);

                // Convert date to UTC for PostgreSQL
                model.TargetImplementationDate = DateTime.SpecifyKind(model.TargetImplementationDate, DateTimeKind.Utc);

                // Save File
                if (model.Attachment != null && model.Attachment.Length > 0)
                {
                    // AppFiles/4mforms (outside wwwroot)
                    var basePath = Path.Combine(_env.ContentRootPath, "AppFiles", "4mforms");
                    Directory.CreateDirectory(basePath);

                    var safeFileName = Path.GetFileName(model.Attachment.FileName);
                    var fileName = $"{model.ControlNumber}_{safeFileName}";
                    var filePath = Path.Combine(basePath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await model.Attachment.CopyToAsync(stream);
                    }

                    // Store RELATIVE path (DB-friendly)
                    model.AttachmentPath = Path.Combine("AppFiles", "4mforms", fileName);

                    if (model.AttachmentPath.Length > 255)
                        model.AttachmentPath = model.AttachmentPath.Substring(0, 255);
                }


                model.Status = "-"; //initial status
                model.PdfAttachmentPath = "";


                _dbContext._4mForms.Add(model);
                await _dbContext.SaveChangesAsync();

                return Json(new { success = true, controlNumber = model.ControlNumber });
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

        // 4M Form Batch Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchUpload(IFormFile excelFile, IFormFile attachmentFile)
        {
            if (excelFile == null || attachmentFile == null)
                return Json(new { success = false, message = "Both Excel files are required." });

            // Validate Excel extensions
            bool IsExcel(IFormFile file)
            {
                var ext = Path.GetExtension(file.FileName).ToLower();
                return ext == ".xls" || ext == ".xlsx";
            }

            if (!IsExcel(excelFile) || !IsExcel(attachmentFile))
                return Json(new { success = false, message = "Only Excel files are allowed." });

            try
            {
                // ================= GENERATE ONE CONTROL NUMBER =================
                var controlNumber = await GenerateControlNumber();

                // ================= SAVE ATTACHMENT (ONCE) =================
                var uploadsFolder = Path.Combine(_env.ContentRootPath, "AppFiles", "4mforms");
                Directory.CreateDirectory(uploadsFolder);

                var safeFileName = Path.GetFileName(attachmentFile.FileName);
                var attachmentFileName = $"{controlNumber}_{safeFileName}";
                var attachmentPath = Path.Combine(uploadsFolder, attachmentFileName);

                using (var stream = new FileStream(attachmentPath, FileMode.Create, FileAccess.Write))
                {
                    await attachmentFile.CopyToAsync(stream);
                }

                // Save relative path (for DB) — use ContentRootPath reference
                var savedAttachmentPath = Path.Combine("AppFiles", "4mforms", attachmentFileName);

                // Truncate if too long
                if (savedAttachmentPath.Length > 255)
                    savedAttachmentPath = savedAttachmentPath.Substring(0, 255);


                // ================= READ EXCEL =================
                var records = new List<_4mForm>();
                int skippedRows = 0;

                using var streamExcel = excelFile.OpenReadStream();
                using var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(streamExcel);

                reader.Read(); // Skip header row

                while (reader.Read())
                {
                    string GetStringSafe(int col) => reader.GetValue(col)?.ToString()?.Trim() ?? string.Empty;

                    DateTime GetDateTimeSafe(int col)
                    {
                        var value = reader.GetValue(col);
                        if (value == null) return DateTime.MinValue;

                        return value switch
                        {
                            DateTime dt => DateTime.SpecifyKind(dt, DateTimeKind.Utc),
                            double oa => DateTime.FromOADate(oa),
                            string s => DateTime.TryParse(s, out var parsed) ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc) : DateTime.MinValue,
                            _ => DateTime.MinValue
                        };
                    }

                    // Read fields
                    var supplierSubmissionDate = GetDateTimeSafe(0);
                    var targetImplementationDate = GetDateTimeSafe(1);
                    var companyName = GetStringSafe(2);
                    var partCode = GetStringSafe(5);
                    var partName = GetStringSafe(6);

                    // Skip row if any required field is missing or invalid
                    if (string.IsNullOrEmpty(companyName) ||
                        string.IsNullOrEmpty(partCode) ||
                        string.IsNullOrEmpty(partName) ||
                        supplierSubmissionDate == DateTime.MinValue ||
                        targetImplementationDate == DateTime.MinValue)
                    {
                        skippedRows++;
                        continue;
                    }

                    var form = new _4mForm
                    {
                        ControlNumber = controlNumber,
                        CompanyName = companyName,
                        SupplierPIC = GetStringSafe(3),
                        TypeOfChange = GetStringSafe(4),
                        PartCode = partCode,
                        PartName = partName,
                        ChangeReason = GetStringSafe(7),
                        PQCPIC = GetStringSafe(8),
                        SupplierSubmissionDate = supplierSubmissionDate,
                        TargetImplementationDate = targetImplementationDate,
                        AttachmentPath = savedAttachmentPath,
                        Status = "-"
                    };

                    records.Add(form);
                }

                if (!records.Any())
                    return Json(new { success = false, message = "Excel file contains no complete data to insert." });

                // ================= SAVE BATCH =================
                _dbContext._4mForms.AddRange(records);
                await _dbContext.SaveChangesAsync();

                // ================= RETURN JSON WITH ALERT INFO =================
                string message = $"Batch upload successful!<br/>Control Number: <strong>{controlNumber}</strong><br/>Inserted: {records.Count}";
                if (skippedRows > 0)
                    message += $"<br/>Skipped incomplete rows: {skippedRows}";

                return Json(new
                {
                    success = true,
                    message,
                    controlNumber,
                    totalInserted = records.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error: {ex.InnerException?.Message ?? ex.Message}"
                });
            }
        }



        public IActionResult PreviewPdf(int id)
        {
            var form = _dbContext._4mForms.FirstOrDefault(f => f.Id == id);
            if (form == null || string.IsNullOrEmpty(form.AttachmentPath))
                return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, "4mforms", Path.GetFileName(form.AttachmentPath));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, "application/pdf");
        }

        // Download attached 4M form
        [HttpGet]
        public IActionResult DownloadAttachmentPDF(int id)
        {
            var form = _dbContext._4mForms.FirstOrDefault(f => f.Id == id);

            if (form == null || string.IsNullOrEmpty(form.AttachmentPath))
                return NotFound("File not found.");

            // Combine with ContentRootPath since AppFiles is outside wwwroot
            var physicalPath = Path.Combine(_env.ContentRootPath, form.PdfAttachmentPath);

            if (!System.IO.File.Exists(physicalPath))
                return NotFound($"File not found on server: {physicalPath}");

            var fileName = Path.GetFileName(physicalPath);

            // Detect content type based on file extension
            var extension = Path.GetExtension(fileName).ToLower();
            string contentType = extension switch
            {
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                _ => "application/octet-stream"
            };

            return PhysicalFile(
                physicalPath,
                contentType,
                fileName
            );
        }

        // Download attached 4M form
        [HttpGet]
        public IActionResult DownloadAttachmentExcel(int id)
        {
            var form = _dbContext._4mForms.FirstOrDefault(f => f.Id == id);

            if (form == null || string.IsNullOrEmpty(form.AttachmentPath))
                return NotFound("File not found.");

            // Combine with ContentRootPath since AppFiles is outside wwwroot
            var physicalPath = Path.Combine(_env.ContentRootPath, form.AttachmentPath);

            if (!System.IO.File.Exists(physicalPath))
                return NotFound($"File not found on server: {physicalPath}");

            var fileName = Path.GetFileName(physicalPath);

            // Detect content type based on file extension
            var extension = Path.GetExtension(fileName).ToLower();
            string contentType = extension switch
            {
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                _ => "application/octet-stream"
            };

            return PhysicalFile(
                physicalPath,
                contentType,
                fileName
            );
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update4mFormPdf(int FormId)
        {
            // Get the first form to retrieve the control number
            var firstForm = await _dbContext._4mForms.FirstOrDefaultAsync(f => f.Id == FormId);
            if (firstForm == null)
                return Json(new { success = false, message = "Form not found" });

            // Get ALL forms with the same control number
            var formsToUpdate = await _dbContext._4mForms
                .Where(f => f.ControlNumber == firstForm.ControlNumber)
                .ToListAsync();

            if (!formsToUpdate.Any())
                return Json(new { success = false, message = "No forms found with this control number" });

            var file = Request.Form.Files["UpdatedPdf"];
            if (file == null || file.Length == 0)
                return Json(new { success = false, message = "No file uploaded" });

            var allowedExtensions = new[] { ".xls", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return Json(new
                {
                    success = false,
                    message = "Only Excel files (.xls, .xlsx) are allowed"
                });
            }

            if (file.Length > 10 * 1024 * 1024)
                return Json(new { success = false, message = "File too large (max 10MB)" });

            // AppFiles/4mforms folder outside wwwroot
            string uploadsFolder = Path.Combine(_env.ContentRootPath, "AppFiles", "4mforms");
            Directory.CreateDirectory(uploadsFolder);

            string safeFileName = Path.GetFileName(file.FileName);
            string uniqueFileName = $"{firstForm.ControlNumber}_{safeFileName}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                // ================= DELETE OLD FILE (if exists) =================
                if (!string.IsNullOrEmpty(firstForm.AttachmentPath))
                {
                    string oldPath = Path.Combine(_env.ContentRootPath, firstForm.AttachmentPath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // ================= SAVE NEW FILE =================
                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                // ================= UPDATE ALL FORMS WITH SAME CONTROL NUMBER =================
                // Save relative path for DB
                string newAttachmentPath = Path.Combine("AppFiles", "4mforms", uniqueFileName);

                foreach (var form in formsToUpdate)
                {
                    form.AttachmentPath = newAttachmentPath;
                    form.Status = "FOR ROHS PIC APPROVAL";
                    form.HasReply = true;
                    _dbContext.Update(form);
                }

                await _dbContext.SaveChangesAsync();

                // ================= GET ROHS PIC EMAIL(S) =================
                var rohsEmails = await _dbContext.Users
                    .Where(u =>
                        u.ApproverRole == "ROHS PIC" &&
                        !string.IsNullOrEmpty(u.Email))
                    .Select(u => u.Email)
                    .ToListAsync();

                if (!rohsEmails.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No active ROHS PIC email found. Please contact system administrator."
                    });
                }

                // ================= EMAIL =================
                string subject = $"4M Change Request for ROHS Approval – {firstForm.ControlNumber}";
                string formLink = $"http://apbiphbpswb01/PCS/_4mForm/FourMForm?tab=form-approval&id={firstForm.Id}";

                // Build table rows for ALL items
                var tableRows = string.Join("", formsToUpdate.Select(form => $@"
            <tr>
                <td style='text-align:center;'>{form.CompanyName}</td>
                <td style='text-align:center;'>
                    <a href='{formLink}' target='_blank'>{form.ControlNumber}</a>
                </td>
                <td style='text-align:center;'>{form.PartCode}</td>
                <td style='text-align:center;'>{form.PartName}</td>
                <td>{form.ChangeReason}</td>
            </tr>"));

                string body = $@"
            <p>Dear ROHS PICs,</p>

            <p>Good day!</p>

            <p>
                Attached herewith is the <strong>4M Manufacturing Process Change Request</strong>
                submitted for <strong>ROHS review and approval</strong>.
            </p>

            <p>Please find the details below:</p>

            <table border='1' cellpadding='8' cellspacing='0'
                   style='border-collapse:collapse;width:100%;font-family:Arial;font-size:13px'>
                <tr style='background-color:#f2f2f2'>
                    <th>Company Name</th>
                    <th>Control Number</th>
                    <th>Part Code</th>
                    <th>Part Name</th>
                    <th>Change Reason</th>
                </tr>
                {tableRows}
            </table>

            <br/>

            <p>
                Kindly review the attached document and proceed with the necessary evaluation.
                Should you require further information or clarification, please coordinate
                with the assigned PQC PIC or supplier.
            </p>

            <p>Thank you.</p>

            <br/>

            <p>
                <strong>Parts Control System Notification</strong><br/>
                <i>This is a system-generated email. Please do not reply.</i>
            </p>";

                // Send email with attachment
                var attachments = new List<(byte[] fileBytes, string fileName)>
        {
            (await System.IO.File.ReadAllBytesAsync(filePath), Path.GetFileName(filePath))
        };

                foreach (var email in rohsEmails)
                {
                    await _mailService.SendEmailAsync(email, subject, body, attachments);
                }

                return Json(new
                {
                    success = true,
                    message = $"Updated {formsToUpdate.Count} form(s) with control number {firstForm.ControlNumber} and sent email to ROHS PIC."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error: " + ex.Message
                });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _4mFormApproval(int id)
        {
            try
            {
                // ================= GET USER CLAIMS =================
                var approverRole = User.FindFirst("ApproverRole")?.Value;
                var approverName = User.FindFirst("FullName")?.Value ??
                                   User.FindFirst(ClaimTypes.Name)?.Value ??
                                   User.Identity.Name;

                if (string.IsNullOrEmpty(approverRole))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Approver role not found. Please re-login."
                    });
                }

                // ================= GET FIRST FORM =================
                var firstForm = await _dbContext._4mForms
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (firstForm == null)
                    return Json(new { success = false, message = "Form not found." });

                // ================= GET ALL FORMS WITH SAME CONTROL NUMBER =================
                var formsToUpdate = await _dbContext._4mForms
                    .Where(f => f.ControlNumber == firstForm.ControlNumber)
                    .ToListAsync();

                if (!formsToUpdate.Any())
                    return Json(new { success = false, message = "No forms found with this control number." });

                // ================= CHECK EXCEL FILE =================
                if (string.IsNullOrEmpty(firstForm.AttachmentPath))
                {
                    return Json(new
                    {
                        success = false,
                        message = "No Form attachment found."
                    });
                }

                string filePath = Path.Combine(_env.ContentRootPath, firstForm.AttachmentPath);
                if (!System.IO.File.Exists(filePath))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Attached Form file does not exist on the server."
                    });
                }

                // ================= UPDATE EXCEL WITH APPROVER NAME =================
                var excelService = new ExcelApprovalService();
                var (excelSuccess, excelError) = await excelService.AddApproverToExcel(
                    filePath,
                    approverName,
                    approverRole
                );

                if (!excelSuccess)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Excel Update Error: {excelError}"
                    });
                }

                // ================= DETERMINE NEXT STATUS =================
                string nextStatus = "";
                string nextApproverRole = "";
                string emailSubjectSuffix = "";
                bool isFinalApproval = false;

                switch (approverRole)
                {
                    case "ROHS PIC":
                        nextStatus = "FOR PSUG PIC APPROVAL";
                        nextApproverRole = "PSUG PIC";
                        emailSubjectSuffix = "PSUG Approval";
                        break;

                    case "PSUG PIC":
                        nextStatus = "FOR SUPERVISOR APPROVAL";
                        nextApproverRole = "SUPERVISOR";
                        emailSubjectSuffix = "Supervisor Approval";
                        break;

                    case "SUPERVISOR":
                        nextStatus = "FOR MANAGER APPROVAL";
                        nextApproverRole = "MANAGER";
                        emailSubjectSuffix = "Manager Approval";
                        break;

                    case "MANAGER":
                        nextStatus = "APPROVED";
                        nextApproverRole = null;
                        emailSubjectSuffix = null;
                        isFinalApproval = true;
                        break;

                    default:
                        return Json(new
                        {
                            success = false,
                            message = "You are not authorized to approve this form."
                        });
                }

                // ================= UPDATE ALL FORMS =================
                foreach (var form in formsToUpdate)
                {
                    form.Status = nextStatus;
                    if (isFinalApproval)
                        form.ApprovedDate = DateTime.UtcNow;

                    _dbContext.Update(form);
                }

                await _dbContext.SaveChangesAsync();

                // ================= IF FINAL APPROVAL, SAVE PDF =================
                if (isFinalApproval)
                {
                    string pdfFolder = Path.Combine(_env.ContentRootPath, "PDFForms");
                    Directory.CreateDirectory(pdfFolder);

                    // Set GemBox license (FREE version works for up to 150 rows)
                    GemBox.Spreadsheet.SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");

                    foreach (var form in formsToUpdate)
                    {
                        string excelPath = Path.Combine(_env.ContentRootPath, form.AttachmentPath);
                        string pdfPath = Path.Combine(pdfFolder, Path.GetFileNameWithoutExtension(excelPath) + ".pdf");

                        try
                        {
                            // Load Excel file
                            var workbook = GemBox.Spreadsheet.ExcelFile.Load(excelPath);

                            // Save as PDF
                            workbook.Save(pdfPath, GemBox.Spreadsheet.SaveOptions.PdfDefault);

                            // Save relative PDF path in the form for future email sending
                            form.PdfAttachmentPath = Path.Combine("PDFForms", Path.GetFileName(pdfPath));
                            _dbContext.Update(form);
                        }
                        catch (Exception exPdf)
                        {
                            Console.WriteLine($"Error saving PDF for {form.ControlNumber}: {exPdf.Message}");
                        }
                    }

                    await _dbContext.SaveChangesAsync();

                    return Json(new
                    {
                        success = true,
                        message = $"All {formsToUpdate.Count} form(s) fully approved. PDF saved and Excel updated."
                    });
                }


                // ================= GET NEXT APPROVER EMAILS =================
                var approverEmails = await _dbContext.Users
                    .Where(u => u.ApproverRole == nextApproverRole && !string.IsNullOrEmpty(u.Email))
                    .Select(u => u.Email)
                    .ToListAsync();

                if (!approverEmails.Any())
                    return Json(new
                    {
                        success = false,
                        message = $"No active {nextApproverRole} email found."
                    });

                // ================= EMAIL CONTENT =================
                string subject =
                    $"4M Change Request for {emailSubjectSuffix} – {firstForm.ControlNumber}";

                string formLink =
                    $"http://apbiphbpswb01/PCS/_4mForm/FourMForm?tab=form-approval&id={firstForm.Id}";

                var tableRows = string.Join("", formsToUpdate.Select(form => $@"
                    <tr>
                        <td style='text-align:center;'>{form.CompanyName}</td>
                        <td style='text-align:center;'>
                            <a href='{formLink}' target='_blank'>{form.ControlNumber}</a>
                        </td>
                        <td style='text-align:center;'>{form.PartCode}</td>
                        <td style='text-align:center;'>{form.PartName}</td>
                        <td>{form.ChangeReason}</td>
                    </tr>"));

                string body = $@"
                    <p>Dear {nextApproverRole},</p>
                    <p>Good day,</p>
                    <p>A <strong>4M Manufacturing Process Change Request</strong> is pending for your review.</p>
                    <table border='1' cellpadding='8' cellspacing='0'
                           style='border-collapse:collapse;width:100%;font-family:Arial;font-size:13px'>
                        <tr style='background-color:#f2f2f2'>
                            <th>Company Name</th>
                            <th>Control Number</th>
                            <th>Part Code</th>
                            <th>Part Name</th>
                            <th>Change Reason</th>
                        </tr>
                        {tableRows}
                    </table>
                    <br/>
                    <p>Please review and proceed with approval.</p>
                    <p><strong>Parts Control System Notification</strong><br/><i>This is a system-generated email.</i></p>";

                // ================= ATTACH ORIGINAL EXCEL =================
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var attachments = new List<(byte[] fileBytes, string fileName)>
                {
                    (fileBytes, Path.GetFileName(filePath))
                };

                // Send ONE email to all recipients at once
                await _mailService.SendEmailAsync(
                    approverEmails, // pass the whole list
                    subject,
                    body,
                    attachments
                );

                return Json(new
                {
                    success = true,
                    message = $"All {formsToUpdate.Count} form(s) sent for {emailSubjectSuffix}. Excel updated with approver info."
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }




        public IActionResult ApprovalMonitoring()
        {
            var data = _dbContext._4mForms
                .OrderByDescending(x => x.SupplierSubmissionDate)
                .ToList();

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                // ================= GET USER CLAIMS =================
                var approverRole = User.FindFirst("ApproverRole")?.Value;
                var approverName = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity.Name;

                if (string.IsNullOrEmpty(approverRole))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Approver role not found. Please re-login."
                    });
                }

                // ================= GET FIRST FORM =================
                var firstForm = await _dbContext._4mForms
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (firstForm == null)
                {
                    return Json(new { success = false, message = "Form not found." });
                }

                // ================= GET ALL FORMS WITH SAME CONTROL NUMBER =================
                var formsToUpdate = await _dbContext._4mForms
                    .Where(f => f.ControlNumber == firstForm.ControlNumber)
                    .ToListAsync();

                if (!formsToUpdate.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No forms found with this control number."
                    });
                }

                // ================= UPDATE STATUS TO REJECTED =================
                foreach (var form in formsToUpdate)
                {
                    form.Status = "REJECTED by " + approverName + " " + DateTime.UtcNow;
                    //form.RejectedBy = approverName;          // optional column
          

                    _dbContext.Update(form);
                }

                await _dbContext.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"All {formsToUpdate.Count} form(s) with control number {firstForm.ControlNumber} have been rejected."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(int id)
        {
            try
            {
                // ================= GET FIRST FORM =================
                var firstForm = await _dbContext._4mForms
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (firstForm == null)
                    return Json(new { success = false, message = "Form not found." });

                // ================= GET ALL FORMS WITH SAME CONTROL NUMBER =================
                var formsToSend = await _dbContext._4mForms
                    .Where(f => f.ControlNumber == firstForm.ControlNumber)
                    .ToListAsync();

                if (!formsToSend.Any())
                    return Json(new { success = false, message = "No forms found with this control number." });

                // ================= FILTER UNSENT FORMS =================
                var unsentForms = formsToSend.Where(f => !f.IsEmailSent).ToList();
                if (!unsentForms.Any())
                    return Json(new { success = false, message = "Email has already been sent for this form." });

                // ================= BUILD EMAIL CONTENT =================
                string subject = $"4M Manufacturing Process Change Request – {firstForm.ControlNumber}";
                string formLink = $"http://apbiphbpswb01/PCS/_4mForm/FourMForm?tab=form-sending&id={firstForm.Id}";
                string companyName = firstForm.CompanyName;

                var tableRows = string.Join("", unsentForms.Select(form => $@"
                    <tr>
                        <td style='text-align:center;'>{form.CompanyName}</td>
                        <td style='text-align:center;'>{form.ControlNumber}</td>
                        <td style='text-align:center;'>{form.PartCode}</td>
                        <td style='text-align:center;'>{form.PartName}</td>
                        <td>{form.ChangeReason}</td>
                    </tr>"));

                string body = $@"
                    <p>To <strong>{companyName}</strong>,</p>

                    <p>Good day!</p>

                    <p>
                        Attached herewith is the <strong>4M Manufacturing Process Change Request</strong>
                        submitted for below parts.
                    </p>

                    <p>Please refer to the details below for additional information:</p>

                    <table border='1' cellpadding='8' cellspacing='0'
                           style='border-collapse:collapse;width:100%;font-family:Arial;font-size:13px'>
                        <tr style='background-color:#f2f2f2'>
                            <th>Company Name</th>
                            <th>Control Number</th>
                            <th>Part Code</th>
                            <th>Part Name</th>
                            <th>Change Reason</th>
                        </tr>
                        {tableRows}
                    </table>

                    <br/>

                    <p>
                       For our reference only.
                    </p>

                    <p>Thank you.</p>

                    <br/>

                    <p>
                        <strong>Parts Control System Notification</strong><br/>
                        <i>This is a system-generated email. Please do not reply.</i>
                    </p>";

                // ================= ATTACHMENTS =================
                var attachments = new List<(byte[] fileBytes, string fileName)>();

                foreach (var form in unsentForms)
                {
                    if (!string.IsNullOrEmpty(form.AttachmentPath))
                    {
                        string fullPath = Path.Combine(_env.ContentRootPath, form.PdfAttachmentPath);

                        if (System.IO.File.Exists(fullPath))
                        {
                            // Attach the file directly, no conversion
                            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
                            attachments.Add((fileBytes, Path.GetFileName(fullPath)));
                        }

                        // update IsEmailSent
                        form.IsEmailSent = true;
                        _dbContext.Update(form);
                    }

                }

                // Save database updates after preparing attachments
                await _dbContext.SaveChangesAsync();



                await _dbContext.SaveChangesAsync();

                // ================= GET SUPPLIER EMAILS BASED ON COMPANY NAME =================
                var companyNames = unsentForms.Select(f => f.CompanyName).Distinct().ToList();
                var recipientEmails = await _dbContext.Suppliers
                    .Where(s => companyNames.Contains(s.SupplierName) && !string.IsNullOrEmpty(s.Email))
                    .Select(s => s.Email)
                    .ToListAsync();

                if (!recipientEmails.Any())
                    return Json(new { success = false, message = "No supplier emails found for this form." });

                // ================= SEND EMAIL =================
                foreach (var email in recipientEmails)
                {
                    await _mailService.SendEmailAsync(email, subject, body, attachments);
                }

                return Json(new
                {
                    success = true,
                    message = $"Email sent successfully to {companyName}." //check this 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error sending email: {ex.Message}" });
            }
        }


    }
}
