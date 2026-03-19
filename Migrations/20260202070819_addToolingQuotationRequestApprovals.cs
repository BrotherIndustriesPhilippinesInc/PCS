using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class addToolingQuotationRequestApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "toolingQuotationRequestApprovals",
                newName: "remarks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "toolingQuotationRequestApprovals",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TargetApprovalDate",
                table: "toolingQuotationRequestApprovals",
                newName: "target_approval_date");

            migrationBuilder.RenameColumn(
                name: "LimitDate",
                table: "toolingQuotationRequestApprovals",
                newName: "limit_date");

            migrationBuilder.RenameColumn(
                name: "ActualApprovalDate",
                table: "toolingQuotationRequestApprovals",
                newName: "actual_approval_date");

            migrationBuilder.AddColumn<string>(
                name: "control_no",
                table: "toolingQuotationRequestApprovals",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "control_no",
                table: "toolingQuotationRequestApprovals");

            migrationBuilder.RenameColumn(
                name: "remarks",
                table: "toolingQuotationRequestApprovals",
                newName: "Remarks");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "toolingQuotationRequestApprovals",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "target_approval_date",
                table: "toolingQuotationRequestApprovals",
                newName: "TargetApprovalDate");

            migrationBuilder.RenameColumn(
                name: "limit_date",
                table: "toolingQuotationRequestApprovals",
                newName: "LimitDate");

            migrationBuilder.RenameColumn(
                name: "actual_approval_date",
                table: "toolingQuotationRequestApprovals",
                newName: "ActualApprovalDate");
        }
    }
}
