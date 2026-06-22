using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadTimeToChangeMaterialMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "section",
                table: "change_material_processes",
                newName: "Section");

            migrationBuilder.RenameColumn(
                name: "remarks",
                table: "change_material_processes",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "activity",
                table: "change_material_processes",
                newName: "Activity");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "change_material_processes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "target_date",
                table: "change_material_processes",
                newName: "TargetDate");

            migrationBuilder.RenameColumn(
                name: "step_order",
                table: "change_material_processes",
                newName: "StepOrder");

            migrationBuilder.RenameColumn(
                name: "process_step",
                table: "change_material_processes",
                newName: "ProcessStep");

            migrationBuilder.RenameColumn(
                name: "input_by",
                table: "change_material_processes",
                newName: "InputBy");

            migrationBuilder.RenameColumn(
                name: "current_process",
                table: "change_material_processes",
                newName: "CurrentProcess");

            migrationBuilder.RenameColumn(
                name: "create_date",
                table: "change_material_processes",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "control_number",
                table: "change_material_processes",
                newName: "ControlNumber");

            migrationBuilder.RenameColumn(
                name: "actual_date",
                table: "change_material_processes",
                newName: "ActualDate");

            migrationBuilder.RenameIndex(
                name: "IX_change_material_processes_control_number_process_step",
                table: "change_material_processes",
                newName: "IX_change_material_processes_ControlNumber_ProcessStep");

            migrationBuilder.RenameColumn(
                name: "section",
                table: "change_material_process_mappings",
                newName: "Section");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "change_material_process_mappings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "step_order",
                table: "change_material_process_mappings",
                newName: "StepOrder");

            migrationBuilder.RenameColumn(
                name: "process_step",
                table: "change_material_process_mappings",
                newName: "ProcessStep");

            migrationBuilder.CreateTable(
                name: "view_change_material_monitoring",
                columns: table => new
                {
                    ControlNumber = table.Column<string>(type: "text", nullable: false),
                    PartName = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Supplier = table.Column<string>(type: "text", nullable: false),
                    Section = table.Column<string>(type: "text", nullable: false),
                    ChildPartcode = table.Column<string>(type: "text", nullable: false),
                    BIPHMoldNo = table.Column<string>(type: "text", nullable: false),
                    SupplierMoldNo = table.Column<string>(type: "text", nullable: false),
                    MoldMaker = table.Column<string>(type: "text", nullable: false),
                    ToolingManagement = table.Column<string>(type: "text", nullable: false),
                    MaterialLoaTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaterialLoaActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenPhTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenPhActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenEvalTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    KatakenEvalActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QaEvalTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QaEvalActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeEvalTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeEvalActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TestRunTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TestRunActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ImplDateTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ImplDateActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstDelTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FirstDelActualDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentProcess = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "view_change_material_monitoring");

            migrationBuilder.RenameColumn(
                name: "Section",
                table: "change_material_processes",
                newName: "section");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "change_material_processes",
                newName: "remarks");

            migrationBuilder.RenameColumn(
                name: "Activity",
                table: "change_material_processes",
                newName: "activity");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "change_material_processes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TargetDate",
                table: "change_material_processes",
                newName: "target_date");

            migrationBuilder.RenameColumn(
                name: "StepOrder",
                table: "change_material_processes",
                newName: "step_order");

            migrationBuilder.RenameColumn(
                name: "ProcessStep",
                table: "change_material_processes",
                newName: "process_step");

            migrationBuilder.RenameColumn(
                name: "InputBy",
                table: "change_material_processes",
                newName: "input_by");

            migrationBuilder.RenameColumn(
                name: "CurrentProcess",
                table: "change_material_processes",
                newName: "current_process");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "change_material_processes",
                newName: "create_date");

            migrationBuilder.RenameColumn(
                name: "ControlNumber",
                table: "change_material_processes",
                newName: "control_number");

            migrationBuilder.RenameColumn(
                name: "ActualDate",
                table: "change_material_processes",
                newName: "actual_date");

            migrationBuilder.RenameIndex(
                name: "IX_change_material_processes_ControlNumber_ProcessStep",
                table: "change_material_processes",
                newName: "IX_change_material_processes_control_number_process_step");

            migrationBuilder.RenameColumn(
                name: "Section",
                table: "change_material_process_mappings",
                newName: "section");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "change_material_process_mappings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StepOrder",
                table: "change_material_process_mappings",
                newName: "step_order");

            migrationBuilder.RenameColumn(
                name: "ProcessStep",
                table: "change_material_process_mappings",
                newName: "process_step");
        }
    }
}
