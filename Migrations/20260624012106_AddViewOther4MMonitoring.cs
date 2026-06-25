using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddViewOther4MMonitoring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LeadTime",
                table: "change_material_process_mappings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.UpdateData(
                table: "Other4MProcessMappings",
                keyColumn: "Id",
                keyValue: 11,
                column: "ProcessStep",
                value: "Test Run");

            migrationBuilder.UpdateData(
                table: "Other4MProcessMappings",
                keyColumn: "Id",
                keyValue: 12,
                column: "ProcessStep",
                value: "Implementation Date");

            migrationBuilder.UpdateData(
                table: "Other4MProcessMappings",
                keyColumn: "Id",
                keyValue: 13,
                column: "ProcessStep",
                value: "First Delivery Date");

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 1,
                column: "LeadTime",
                value: 21);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 2,
                column: "LeadTime",
                value: 0);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 3,
                column: "LeadTime",
                value: 7);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 4,
                column: "LeadTime",
                value: 10);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 5,
                column: "LeadTime",
                value: 10);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 6,
                column: "LeadTime",
                value: 2);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 7,
                column: "LeadTime",
                value: 0);

            migrationBuilder.UpdateData(
                table: "change_material_process_mappings",
                keyColumn: "Id",
                keyValue: 8,
                column: "LeadTime",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "LeadTime",
                table: "change_material_process_mappings",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Other4MProcessMappings",
                keyColumn: "Id",
                keyValue: 11,
                column: "ProcessStep",
                value: "TEST RUN");

            migrationBuilder.UpdateData(
                table: "Other4MProcessMappings",
                keyColumn: "Id",
                keyValue: 12,
                column: "ProcessStep",
                value: "IMPLEMENTATION DATE");

            migrationBuilder.UpdateData(
                table: "Other4MProcessMappings",
                keyColumn: "Id",
                keyValue: 13,
                column: "ProcessStep",
                value: "FIRST DELIVERY DATE");

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
    }
}
