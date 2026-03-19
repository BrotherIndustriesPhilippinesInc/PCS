using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class ToolingPOIssuance_change_table_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MP2ToolingPoIssuance",
                table: "MP2ToolingPoIssuance");

            migrationBuilder.RenameTable(
                name: "MP2ToolingPoIssuance",
                newName: "mp2_tooling_po_issuance");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mp2_tooling_po_issuance",
                table: "mp2_tooling_po_issuance",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_mp2_tooling_po_issuance",
                table: "mp2_tooling_po_issuance");

            migrationBuilder.RenameTable(
                name: "mp2_tooling_po_issuance",
                newName: "MP2ToolingPoIssuance");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MP2ToolingPoIssuance",
                table: "MP2ToolingPoIssuance",
                column: "Id");
        }
    }
}
