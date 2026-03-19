using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class addActivityAndReasonOfChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "activity",
                table: "import_data",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason_of_change",
                table: "import_data",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activity",
                table: "import_data");

            migrationBuilder.DropColumn(
                name: "reason_of_change",
                table: "import_data");
        }
    }
}
