using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDelayToActivityCurrentProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "LeadTime",
                table: "change_material_process_mappings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "is_delay",
                table: "activity_current_process",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 1,
                column: "LeadTime",
                value: 21m);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 2,
                column: "LeadTime",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 3,
                column: "LeadTime",
                value: 7m);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 4,
                column: "LeadTime",
                value: 10m);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 5,
                column: "LeadTime",
                value: 10m);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 6,
                column: "LeadTime",
                value: 2m);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 7,
                column: "LeadTime",
                value: 0m);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 8,
                column: "LeadTime",
                value: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeadTime",
                table: "change_material_process_mappings");

            migrationBuilder.DropColumn(
                name: "is_delay",
                table: "activity_current_process");
        }
    }
}
