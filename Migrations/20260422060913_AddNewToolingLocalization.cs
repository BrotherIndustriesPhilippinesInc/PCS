using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddNewToolingLocalization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "new_tooling_category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ControlNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AssignedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_new_tooling_category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "new_tooling_localization_process",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ControlNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Section = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Activity = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ProcessStep = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Result = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ReferenceNo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    InputBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentProcess = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_new_tooling_localization_process", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "new_tooling_process_mapping",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    ProcessStep = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Section = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_new_tooling_process_mapping", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "new_tooling_process_mapping",
                columns: new[] { "Id", "Category", "ProcessStep", "Section", "StepOrder" },
                values: new object[,]
                {
                    { 1, "Multiple Procurement", "Mold LOA", "MP1-PUR", 1 },
                    { 2, "Multiple Procurement", "Material LOA", "MP1-PUR", 2 },
                    { 3, "Multiple Procurement", "Manual FC to new supplier", "MP2-DOM", 3 },
                    { 4, "Multiple Procurement", "Tooling Transfer Date", "PUR-CTRL", 4 },
                    { 5, "Multiple Procurement", "4M Application Date", "MP1-PUR", 5 },
                    { 6, "Multiple Procurement", "Kataken PH Sample Submission", "IQC", 6 },
                    { 7, "Multiple Procurement", "Kataken PH Sample Approval", "IQC", 7 },
                    { 8, "Multiple Procurement", "DE Sample Received Date", "DE", 8 },
                    { 9, "Multiple Procurement", "DE Sample Approval", "DE", 9 },
                    { 10, "Multiple Procurement", "QA Sample Received Date", "QA", 10 },
                    { 11, "Multiple Procurement", "QA Sample Approval", "QA", 11 },
                    { 12, "Multiple Procurement", "Availability of Parts Packaging Standard", "MP1-PUR", 12 },
                    { 13, "Multiple Procurement", "Open Sourcelist (New Supplier)", "MP1-PUR", 13 },
                    { 14, "Multiple Procurement", "Procurement Type Change", "MP1-PUR", 14 },
                    { 15, "Multiple Procurement", "Test Run PO Request Date", "IQC", 15 },
                    { 16, "Multiple Procurement", "Return of Special Procurement Type", "MP1-PUR", 16 },
                    { 17, "Multiple Procurement", "Test Run PO Date", "MP2-DOM", 17 },
                    { 18, "Multiple Procurement", "Test Run Delivery Date", "IQC", 18 },
                    { 19, "Multiple Procurement", "Test Run Schedule", "IQC", 19 },
                    { 20, "Multiple Procurement", "Test Run Approval Date", "IQC", 20 },
                    { 21, "Multiple Procurement", "Quota Arrangement SAP Input", "MP1-PUR", 21 },
                    { 22, "Multiple Procurement", "Confirmation of Parts Availability", "MP1-PUR", 22 },
                    { 23, "Multiple Procurement", "Updated Price Input", "MP1-PUR", 23 },
                    { 24, "Multiple Procurement", "PO Issuance Date (New Supplier)", "MP2-DOM", 24 },
                    { 25, "Multiple Procurement", "Parts Delivery Date (New Supplier Delivery Date)", "MP2-DOM", 25 },
                    { 26, "Supplier Change", "Mold LOA", "MP1-PUR", 1 },
                    { 27, "Supplier Change", "Material LOA", "MP1-PUR", 2 },
                    { 28, "Supplier Change", "Manual FC to new supplier", "PDC-Loc", 3 },
                    { 29, "Supplier Change", "Tooling Transfer Date", "MP1-PUR", 4 },
                    { 30, "Supplier Change", "4M Application Date", "MP1-PUR", 5 },
                    { 31, "Supplier Change", "Kataken PH Sample Submission", "IQC", 6 },
                    { 32, "Supplier Change", "Kataken PH Sample Approval", "IQC", 7 },
                    { 33, "Supplier Change", "DE Sample Received Date", "DE", 8 },
                    { 34, "Supplier Change", "DE Sample Approval", "DE", 9 },
                    { 35, "Supplier Change", "QA Sample Received Date", "QA", 10 },
                    { 36, "Supplier Change", "QA Sample Approval", "QA", 11 },
                    { 37, "Supplier Change", "Availability of Parts Packaging Standard", "MP1-PUR", 12 },
                    { 38, "Supplier Change", "Open Sourcelist (New Supplier)", "MP1-PUR", 13 },
                    { 39, "Supplier Change", "Test Run PO Date", "MP2-DOM", 14 },
                    { 40, "Supplier Change", "Test Run Delivery Date", "MP2-DOM", 15 },
                    { 41, "Supplier Change", "Test Run Schedule", "IQC", 16 },
                    { 42, "Supplier Change", "Test Run Approval Date", "IQC", 17 },
                    { 43, "Supplier Change", "4M Approval Date", "IQC", 18 },
                    { 44, "Supplier Change", "Simulation of Old Suppliers Stocks", "PDC-Loc", 19 },
                    { 45, "Supplier Change", "SAP Setting Change", "MP1-PUR", 20 },
                    { 46, "Supplier Change", "Final PO Delivery (Date)", "PDC-Loc", 21 },
                    { 47, "Supplier Change", "BLK and FIX Supplier", "MP1-PUR", 22 },
                    { 48, "Supplier Change", "Recosting Date", "MP1-PUR", 23 },
                    { 49, "Supplier Change", "PO Issuance Date (New Supplier)", "PDC-Loc", 24 },
                    { 50, "Supplier Change", "Parts Availability (New Supplier Delivery Date)", "PDC-Loc", 25 },
                    { 51, "Localization", "CH Sample Submission Date", "IQC", 1 },
                    { 52, "Localization", "CH Sample Approval Date", "IQC", 2 },
                    { 53, "Localization", "Drawing Issuance to Supplier", "PC-DCI", 3 },
                    { 54, "Localization", "Tooling Transfer Date", "MP1-PUR", 4 },
                    { 55, "Localization", "4M Application Date", "MP1-PUR", 5 },
                    { 56, "Localization", "Kataken PH Sample Submission", "IQC", 6 },
                    { 57, "Localization", "Kataken PH Sample Approval", "IQC", 7 },
                    { 58, "Localization", "Procurement Type Change", "PC-DCI", 8 },
                    { 59, "Localization", "Open Sourcelist Local", "MP1-PUR", 9 },
                    { 60, "Localization", "DE Sample Received Date", "DE", 10 },
                    { 61, "Localization", "DE Sample Approval", "DE", 11 },
                    { 62, "Localization", "QA Sample Received Date", "QA", 12 },
                    { 63, "Localization", "QA Sample Approval", "QA", 13 },
                    { 64, "Localization", "Availability of Parts Packaging Standard", "MP1-PUR", 14 },
                    { 65, "Localization", "Test Run PO Request Date", "IQC", 15 },
                    { 66, "Localization", "Test Run PO Date", "MP2-DOM", 16 },
                    { 67, "Localization", "Test Run Delivery Date", "MP2-DOM", 17 },
                    { 68, "Localization", "Test Run Schedule", "IQC", 18 },
                    { 69, "Localization", "Test Run Approval Date", "IQC", 19 },
                    { 70, "Localization", "4M Approval Date", "IQC", 20 },
                    { 71, "Localization", "Simulation of Old Suppliers Stocks", "PDC-Loc", 21 },
                    { 72, "Localization", "SAP Setting Change", "MP1-PUR", 22 },
                    { 73, "Localization", "Final PO Delivery (Date)", "PDC-Loc", 23 },
                    { 74, "Localization", "BLK and FIX Supplier", "MP1-PUR", 24 },
                    { 75, "Localization", "Recosting Date", "MP1-PUR", 25 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_new_tooling_category_ControlNumber",
                table: "new_tooling_category",
                column: "ControlNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_new_tooling_localization_process_ControlNumber_Category_Pro~",
                table: "new_tooling_localization_process",
                columns: new[] { "ControlNumber", "Category", "ProcessStep" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "new_tooling_category");

            migrationBuilder.DropTable(
                name: "new_tooling_localization_process");

            migrationBuilder.DropTable(
                name: "new_tooling_process_mapping");
        }
    }
}
