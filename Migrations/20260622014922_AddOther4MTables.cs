using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddOther4MTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Other4MProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ControlNumber = table.Column<string>(type: "text", nullable: false),
                    Section = table.Column<string>(type: "text", nullable: true),
                    Activity = table.Column<string>(type: "text", nullable: false),
                    CurrentProcess = table.Column<string>(type: "text", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    TestRunMeetingTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TestRunMeetingActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenRequestTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenRequestActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenSampleSubmissionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenRequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenSubmissionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenStatus = table.Column<string>(type: "text", nullable: true),
                    KatakenRemarks = table.Column<string>(type: "text", nullable: true),
                    DEReferenceNo = table.Column<string>(type: "text", nullable: true),
                    DEWorkflowSystemNo = table.Column<string>(type: "text", nullable: true),
                    DEPartsReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DEPartsEndorsementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DEActualFinishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DEEvalStatus = table.Column<string>(type: "text", nullable: true),
                    DERemarks = table.Column<string>(type: "text", nullable: true),
                    EEPartsReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EEPartsEndorsementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EEActualFinishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EEEvalStatus = table.Column<string>(type: "text", nullable: true),
                    EERemarks = table.Column<string>(type: "text", nullable: true),
                    QAWorkflowNo = table.Column<string>(type: "text", nullable: true),
                    QATargetDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QAPartsReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QAPartsEndorsementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QAActualFinishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QAEvalStatus = table.Column<string>(type: "text", nullable: true),
                    QARemarks = table.Column<string>(type: "text", nullable: true),
                    ITFActualFinishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ITFStatus = table.Column<string>(type: "text", nullable: true),
                    ITFRemarks = table.Column<string>(type: "text", nullable: true),
                    DeliveryPORequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeliveryPOIssuanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeliveryPOTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TestRunPORequestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TestRunPOIssuanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TestRunNo = table.Column<string>(type: "text", nullable: true),
                    TestRunActualReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TestRunActualFinishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TestResult = table.Column<string>(type: "text", nullable: true),
                    TestRunRemarks = table.Column<string>(type: "text", nullable: true),
                    ImplementationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstDeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InputBy = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Other4MProcesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Other4MProcessMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessStep = table.Column<string>(type: "text", nullable: false),
                    StepOrder = table.Column<int>(type: "integer", nullable: false),
                    LeadTimeDays = table.Column<int>(type: "integer", nullable: true),
                    LeadTimeNote = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Other4MProcessMappings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Other4MProcessMappings",
                columns: new[] { "Id", "LeadTimeDays", "LeadTimeNote", "ProcessStep", "StepOrder" },
                values: new object[,]
                {
                    { 1, 5, "5 days after 4M form received", "Test Run meeting date", 1 },
                    { 2, 1, "1 day after 4M form received", "Kataken Request date", 2 },
                    { 3, null, "Manual Input/0", "Kataken PH Sample Submission", 3 },
                    { 4, 7, "7 days", "Kataken Evaluation Approval", 4 },
                    { 5, 10, "10 days", "DE Evaluation", 5 },
                    { 6, 10, "10 days", "EE Evaluation", 6 },
                    { 7, 10, "10 days", "QA Evaluation", 7 },
                    { 8, 2, "2 days", "ITF Process", 8 },
                    { 9, 1, "1 day after QA evaluation", "Delivery PO Requisition", 9 },
                    { 10, 1, "1 day after delivery of Test Run", "Test Run PO request", 10 },
                    { 11, 2, "2 days", "TEST RUN", 11 },
                    { 12, null, "Depends on DCI implementation / current material stocks depletion", "IMPLEMENTATION DATE", 12 },
                    { 13, null, "Depends on DCI implementation / current material stocks depletion", "FIRST DELIVERY DATE", 13 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Other4MProcesses_ControlNumber",
                table: "Other4MProcesses",
                column: "ControlNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Other4MProcesses");

            migrationBuilder.DropTable(
                name: "Other4MProcessMappings");
        }
    }
}
