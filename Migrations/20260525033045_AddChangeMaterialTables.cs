using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeMaterialTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Tooling PO issuance", "MP2-TOOLING" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Tooling Transfer Date", "MP2-TOOLING" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "4M Application Date", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 7,
                column: "ProcessStep",
                value: "Kataken PH Sample Submission");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Kataken PH Sample Approval", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Availability of Parts Packaging Standard", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 10,
                column: "ProcessStep",
                value: "Open sourcelist (New Supplier) / Updated Price input");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Procurement Type Change", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run PO Request Date", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Return of Special procurement type", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run PO Date", "MP2-DOM" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run Delivery Date", "MP2-DOM" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 16,
                column: "ProcessStep",
                value: "Test Run Schedule");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run Approval Date", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 19,
                column: "ProcessStep",
                value: "Quota Arrangement SAP input");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 20,
                column: "ProcessStep",
                value: "PO issuance Date (New Supplier)");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Multiple Procurement", "Target Usage Date", "MP2", 22 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Mold LOA", 1 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Material LOA", "MP1-PUR", 2 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Manual FC to new supplier", "MP2-DOM", 3 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Tooling PO issuance", "MP2-TOOLING", 4 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Tooling Transfer Date", "MP2-TOOLING", 5 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Application Date", "MP1-PUR", 6 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 29,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Submission", "IQC", 7 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 30,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Approval", "IQC", 8 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 31,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "DE Sample Received Date", "DE", 9 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 32,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "DE Sample Approval", "DE", 10 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 33,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "QA Sample Received Date", "QA", 11 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 34,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "QA Sample Approval", "QA", 12 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 35,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Availability of Parts Packaging Standard", "IQC", 13 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 36,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Open sourcelist (New Supplier)", "MP1-PUR", 14 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 37,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Test Run PO Request Date", 15 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 38,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run PO Date", "MP2-DOM", 16 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 39,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Delivery Date", "MP2-DOM", 17 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 40,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Schedule", "IQC", 18 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 41,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Approval Date", "IQC", 19 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 42,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Approval Date", "IQC", 20 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 43,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Request Simulation to MP2", 21 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 44,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Simulation of Old Suppliers Stocks", "MP2-DOM", 22 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 45,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Final PO Delivery (Date)", "MP2-DOM", 23 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 46,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "SAP Setting Change", "MP1-PUR", 24 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 47,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "BLK and FIX Supplier", "MP1-PUR", 25 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 48,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "Recosting Date", "MP1-PUR", 26 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 49,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "PO issuance Date (New Supplier)", "MP2-DOM", 27 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "Parts Availability (New Supplier Delivery Date)", "MP2-DOM", 28 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "Target Usage Date", "MP2", 29 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Tooling PO Issued Date", "MP2-TOOL", 1 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 53,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Drawing Issuance to Supplier", "PC-DCI", 2 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Tooling Transfer Date", "MP2-TOOL", 3 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Application Date", "MP1-PUR", 4 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 56,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Kataken PH Sample Submission", "IQC", 5 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Kataken PH Sample Approval", 6 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Procurement Type Change", "PC-DCI", 7 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Open Sourcelist Local", "MP1-PUR", 8 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Test Run PO Request Date", 9 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run PO Date", "MP2-DOM", 10 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Delivery Date", "MP2-DOM", 11 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Schedule", "IQC", 12 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Test Run Approval Date", "IQC", 13 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Approval Date", "IQC", 14 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Request Simulation to MP2", 15 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Simulation of Old Suppliers Stocks", "MP2-OVR", 16 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Final PO Delivery (Date)", "MP2-OVR", 17 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "SAP Setting Change", "MP1-PUR", 18 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "BLK and FIX Supplier", "MP1-PUR", 19 });

            migrationBuilder.InsertData(
                table: "new_tooling_process_mapping",
                columns: new[] { "Id", "Category", "LeadTimeDays", "ProcessStep", "Section", "StepOrder" },
                values: new object[,]
                {
                    { 71, "Localization", 0m, "Recosting Date", "MP1-PUR", 20 },
                    { 72, "Localization", 0m, "PO issuance Date (New Supplier)", "MP2-DOM", 21 },
                    { 73, "Localization", 0m, "Parts Availability (New Supplier Delivery Date)", "MP2-DOM", 22 },
                    { 74, "Localization", 0m, "Target Usage Date", "MP1-PUR", 23 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Tooling Transfer Date", "PUR-CTRL" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "4M Application Date", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Kataken PH Sample Submission", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 7,
                column: "ProcessStep",
                value: "Kataken PH Sample Approval");

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
                column: "ProcessStep",
                value: "Procurement Type Change");

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
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Return of Special Procurement Type", "MP1-PUR" });

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
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Test Run Schedule", "IQC" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 16,
                column: "ProcessStep",
                value: "Test Run Approval Date");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "ProcessStep", "Section" },
                values: new object[] { "Quota Arrangement SAP Input", "MP1-PUR" });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 19,
                column: "ProcessStep",
                value: "Updated Price Input");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 20,
                column: "ProcessStep",
                value: "PO Issuance Date (New Supplier)");

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Supplier Change", "Mold LOA", "MP1-PUR", 1 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Material LOA", 2 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Manual FC to new supplier", "PDC-Loc", 3 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Tooling Transfer Date", "MP1-PUR", 4 });

            migrationBuilder.UpdateData(
                table: "new_tooling_process_mapping",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "4M Application Date", "MP1-PUR", 5 });

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
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Test Run Schedule", 16 });

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
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "BLK and FIX Supplier", 22 });

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
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Parts Availability (New Supplier Delivery Date)", "PDC-Loc", 25 });

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
                columns: new[] { "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "Localization", "Tooling Transfer Date", "MP1-PUR", 5 });

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
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "Test Run Schedule", 14 });

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
                columns: new[] { "ProcessStep", "Section", "StepOrder" },
                values: new object[] { "SAP Setting Change", "MP1-PUR", 18 });

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
                columns: new[] { "ProcessStep", "StepOrder" },
                values: new object[] { "BLK and FIX Supplier", 20 });

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
    }
}
