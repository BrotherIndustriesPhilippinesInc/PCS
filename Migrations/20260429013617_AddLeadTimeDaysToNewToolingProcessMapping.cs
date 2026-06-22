using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadTimeDaysToNewToolingProcessMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 75);


            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Availability of Parts Packaging Standard", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Open Sourcelist (New Supplier)", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Procurement Type Change", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run PO Request Date", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 12,
                column: "ProcessStep",
                value: "Return of Special Procurement Type");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run PO Date", "MP2-DOM" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run Delivery Date", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 15,
                column: "ProcessStep",
                value: "Test Run Schedule");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run Approval Date", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Quota Arrangement SAP Input", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Confirmation of Parts Availability", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Updated Price Input", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "PO Issuance Date (New Supplier)", "MP2-DOM" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Parts Delivery Date (New Supplier Delivery Date)", "MP2-DOM" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Category", "ProcessStep", "StepOrder" },
                values: new object[] { "Supplier Change", "Mold LOA", 1 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Category", "ProcessStep", "StepOrder" },
                values: new object[] { "Supplier Change", "Material LOA", 2 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "Manual FC to new supplier", "PDC-Loc", 3 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "Tooling Transfer Date", "MP1-PUR", 4 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "4M Application Date", 5 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Submission", "IQC", 6 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Approval", "IQC", 7 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "DE Sample Received Date", "DE", 8 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "DE Sample Approval", "DE", 9 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "QA Sample Received Date", "QA", 10 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "QA Sample Approval", "QA", 11 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Availability of Parts Packaging Standard", "MP1-PUR", 12 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Open Sourcelist (New Supplier)", "MP1-PUR", 13 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run PO Date", "MP2-DOM", 14 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Delivery Date", "MP2-DOM", 15 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Schedule", "IQC", 16 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Approval Date", "IQC", 17 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Approval Date", "IQC", 18 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Simulation of Old Suppliers Stocks", "PDC-Loc", 19 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "SAP Setting Change", "MP1-PUR", 20 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Final PO Delivery (Date)", "PDC-Loc", 21 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "BLK and FIX Supplier", "MP1-PUR", 22 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Recosting Date", "MP1-PUR", 23 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "PO Issuance Date (New Supplier)", "PDC-Loc", 24 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Parts Availability (New Supplier Delivery Date)", 25 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Localization", "Tooling PO Issued Date", "MP2-TOOL", 1 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Localization", "CH Sample Submission Date", "IQC", 2 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Localization", "CH Sample Approval Date", "IQC", 3 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Localization", "Drawing Issuance to Supplier", "PC-DCI", 4 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Tooling Transfer Date", "MP1-PUR", 5 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Application Date", "MP1-PUR", 6 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Submission", "IQC", 7 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Approval", "IQC", 8 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Procurement Type Change", "PC-DCI", 9 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Open Sourcelist Local", "MP1-PUR", 10 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Test Run PO Request Date", 11 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run PO Date", "MP2-DOM", 12 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Delivery Date", "MP2-DOM", 13 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Schedule", "IQC", 14 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Approval Date", "IQC", 15 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Approval Date", "IQC", 16 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Simulation of Old Suppliers Stocks", "PDC-Loc", 17 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "SAP Setting Change", 18 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Final PO Delivery (Date)", "PDC-Loc", 19 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "BLK and FIX Supplier", "MP1-PUR", 20 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Recosting Date", "MP1-PUR", 21 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Material Master Setting Change", "MP2-DOM", 22 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "PO Issuance Date (New Supplier)", "MP2-DOM", 23 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Parts Availability (New Supplier Delivery Date)", "MP2-DOM", 24 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "DE Sample Received Date", "DE" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "DE Sample Approval", "DE" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "QA Sample Received Date", "QA" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "QA Sample Approval", "QA" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 12,
                column: "ProcessStep",
                value: "Availability of Parts Packaging Standard");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Open Sourcelist (New Supplier)", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Procurement Type Change", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 15,
                column: "ProcessStep",
                value: "Test Run PO Request Date");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Return of Special Procurement Type", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run PO Date", "MP2-DOM" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run Delivery Date", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run Schedule", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run Approval Date", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Quota Arrangement SAP Input", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Category", "ProcessStep", "StepOrder" },
                values: new object[] { "Multiple Procurement", "Confirmation of Parts Availability", 22 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "Category", "ProcessStep", "StepOrder" },
                values: new object[] { "Multiple Procurement", "Updated Price Input", 23 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Multiple Procurement", "PO Issuance Date (New Supplier)", "MP2-DOM", 24 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Multiple Procurement", "Parts Delivery Date (New Supplier Delivery Date)", "MP2-DOM", 25 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Mold LOA", 1 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Material LOA", "MP1-PUR", 2 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Manual FC to new supplier", "PDC-Loc", 3 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Tooling Transfer Date", "MP1-PUR", 4 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Application Date", "MP1-PUR", 5 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Submission", "IQC", 6 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Approval", "IQC", 7 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "DE Sample Received Date", "DE", 8 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "DE Sample Approval", "DE", 9 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "QA Sample Received Date", "QA", 10 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "QA Sample Approval", "QA", 11 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Availability of Parts Packaging Standard", "MP1-PUR", 12 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Open Sourcelist (New Supplier)", "MP1-PUR", 13 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run PO Date", "MP2-DOM", 14 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Delivery Date", "MP2-DOM", 15 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Schedule", "IQC", 16 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Approval Date", "IQC", 17 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Approval Date", "IQC", 18 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Simulation of Old Suppliers Stocks", "PDC-Loc", 19 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "SAP Setting Change", "MP1-PUR", 20 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Final PO Delivery (Date)", 21 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "BLK and FIX Supplier", "MP1-PUR", 22 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "Recosting Date", "MP1-PUR", 23 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "PO Issuance Date (New Supplier)", "PDC-Loc", 24 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "Parts Availability (New Supplier Delivery Date)", "PDC-Loc", 25 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "CH Sample Submission Date", "IQC", 1 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "CH Sample Approval Date", "IQC", 2 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Drawing Issuance to Supplier", "PC-DCI", 3 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Tooling Transfer Date", "MP1-PUR", 4 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Application Date", "MP1-PUR", 5 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Submission", "IQC", 6 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Kataken PH Sample Approval", 7 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Procurement Type Change", "PC-DCI", 8 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Open Sourcelist Local", "MP1-PUR", 9 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "DE Sample Received Date", "DE", 10 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "DE Sample Approval", "DE", 11 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "QA Sample Received Date", "QA", 12 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "QA Sample Approval", "QA", 13 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Availability of Parts Packaging Standard", 14 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run PO Request Date", "IQC", 15 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run PO Date", "MP2-DOM", 16 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Delivery Date", "MP2-DOM", 17 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Schedule", "IQC", 18 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Approval Date", "IQC", 19 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Approval Date", "IQC", 20 });

            migrationBuilder.InsertData(
                table: "new_tooling_process_mapping",
                columns: new[] { "Id", "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[,]
                {
                    { 71, "Localization", "Simulation of Old Suppliers Stocks", "PDC-Loc", 21 },
                    { 72, "Localization", "SAP Setting Change", "MP1-PUR", 22 },
                    { 73, "Localization", "Final PO Delivery (Date)", "PDC-Loc", 23 },
                    { 74, "Localization", "BLK and FIX Supplier", "MP1-PUR", 24 },
                    { 75, "Localization", "Recosting Date", "MP1-PUR", 25 }
                });
        }
    }
}
