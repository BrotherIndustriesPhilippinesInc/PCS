using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class addUpdateActivityData3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "control_number",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "field_name",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "field_value",
                table: "update_activity_data");

            migrationBuilder.RenameColumn(
                name: "next_activity",
                table: "update_activity_data",
                newName: "remarks");

            migrationBuilder.AlterColumn<string>(
                name: "section",
                table: "update_activity_data",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "activity",
                table: "update_activity_data",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<decimal>(
                name: "actual",
                table: "update_activity_data",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "approval_lead_time",
                table: "update_activity_data",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "control_no",
                table: "update_activity_data",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "update_activity_data",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "fabrication_lead_time",
                table: "update_activity_data",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "limit_date",
                table: "update_activity_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "need_no_need",
                table: "update_activity_data",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "parts_shortage_date",
                table: "update_activity_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "remaining_days_until_limit",
                table: "update_activity_data",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "result",
                table: "update_activity_data",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "result_email_date_to_supplier",
                table: "update_activity_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "result_passed_failed",
                table: "update_activity_data",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "target",
                table: "update_activity_data",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "target_mold_usage_date",
                table: "update_activity_data",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "transfer_lead_time",
                table: "update_activity_data",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trf_no",
                table: "update_activity_data",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "update_activity_data",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "actual",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "approval_lead_time",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "control_no",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "fabrication_lead_time",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "limit_date",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "need_no_need",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "parts_shortage_date",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "remaining_days_until_limit",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "result",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "result_email_date_to_supplier",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "result_passed_failed",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "target",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "target_mold_usage_date",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "transfer_lead_time",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "trf_no",
                table: "update_activity_data");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "update_activity_data");

            migrationBuilder.RenameColumn(
                name: "remarks",
                table: "update_activity_data",
                newName: "next_activity");

            migrationBuilder.AlterColumn<string>(
                name: "section",
                table: "update_activity_data",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "activity",
                table: "update_activity_data",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "control_number",
                table: "update_activity_data",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "field_name",
                table: "update_activity_data",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "field_value",
                table: "update_activity_data",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
