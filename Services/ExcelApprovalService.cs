using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PartsControlSystem.Services
{
    public class ExcelApprovalService
    {
        public async Task<(bool success, string errorMessage)> AddApproverToExcel(
            string filePath,
            string approverName,
            string approverRole)
        {
            try
            {
                if (!File.Exists(filePath))
                    return (false, $"File not found: {filePath}");

                if (IsFileLocked(filePath))
                    return (false, "Excel file is currently open or locked. Please close it and try again.");

                if (string.IsNullOrWhiteSpace(approverName))
                    return (false, "Approver name is empty. Please ensure user name is properly configured.");

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = null;

                    worksheet = package.Workbook.Worksheets["JPN_ENG "];
                    if (worksheet == null)
                        worksheet = package.Workbook.Worksheets["JPN_ENG"];

                    if (worksheet == null)
                    {
                        foreach (var ws in package.Workbook.Worksheets)
                        {
                            if (ws.Name.Trim().Equals("JPN_ENG", StringComparison.OrdinalIgnoreCase))
                            {
                                worksheet = ws;
                                break;
                            }
                        }
                    }

                    if (worksheet == null)
                    {
                        var availableSheets = string.Join(", ", package.Workbook.Worksheets.Select(w => $"'{w.Name}'"));
                        return (false, $"JPN_ENG sheet not found. Available sheets: {availableSheets}");
                    }

                    int approvalColumn = GetApprovalColumn(approverRole);
                    if (approvalColumn == 0)
                        return (false, $"Invalid approver role: {approverRole}");

                    int approvalRow = 51; // dynamic row logic can go here if needed
                    // Write initials instead of full name
                    worksheet.Cells[approvalRow, approvalColumn].Value = ConvertToInitialFormat(approverName);

                    await package.SaveAsync();
                    return (true, "Success");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }

        private string ConvertToInitialFormat(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "";

            var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return parts[0]; // single name, return as is

            string firstInitial = parts[0][0] + ".";
            string lastName = parts[^1]; // last element
            return $"{firstInitial} {lastName}";
        }

        private int GetApprovalColumn(string approverRole)
        {
            return approverRole switch
            {
                "ROHS PIC" => 19,
                "PSUG PIC" => 17,
                "SUPERVISOR" => 15,
                "MANAGER" => 11,
                _ => 0
            };
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    stream.Close();
                }
                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}
