using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddChangeMaterialTables1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "change_material_process_mappings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    process_step = table.Column<string>(type: "text", nullable: false),
                    step_order = table.Column<int>(type: "integer", nullable: false),
                    section = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_change_material_process_mappings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "change_material_processes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    control_number = table.Column<string>(type: "text", nullable: false),
                    section = table.Column<string>(type: "text", nullable: false),
                    activity = table.Column<string>(type: "text", nullable: false),
                    process_step = table.Column<string>(type: "text", nullable: false),
                    step_order = table.Column<int>(type: "integer", nullable: false),
                    target_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actual_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    remarks = table.Column<string>(type: "text", nullable: false),
                    current_process = table.Column<string>(type: "text", nullable: false),
                    input_by = table.Column<string>(type: "text", nullable: false),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_change_material_processes", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "change_material_process_mappings",
                columns: new[] { "id", "process_step", "section", "step_order" },
                values: new object[,]
                {
                    { 1, "Material LOA", "MP1", 1 },
                    { 2, "Kataken PH Sample Submission", "IQC", 2 },
                    { 3, "Kataken Evaluation Approval", "IQC", 3 },
                    { 4, "QA Evaluation", "IQC", 4 },
                    { 5, "DE Evaluation", "IQC", 5 },
                    { 6, "Test Run", "IQC", 6 },
                    { 7, "Implementation Date", "MP1", 7 },
                    { 8, "First Delivery Date", "MP1", 8 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_change_material_processes_control_number_process_step",
                table: "change_material_processes",
                columns: new[] { "control_number", "process_step" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "change_material_process_mappings");

            migrationBuilder.DropTable(
                name: "change_material_processes");
        }
    }
}
