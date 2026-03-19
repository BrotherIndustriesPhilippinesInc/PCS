using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class addUpdateMatrix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.CreateTable(
                name: "update_activity_matrix",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    section = table.Column<string>(type: "text", nullable: false),
                    activity = table.Column<string>(type: "text", nullable: false),
                    limit_date = table.Column<bool>(type: "boolean", nullable: false),
                    remaining_days_until_limit = table.Column<bool>(type: "boolean", nullable: false),
                    transfer_lead_time = table.Column<bool>(type: "boolean", nullable: false),
                    fabrication_lead_time = table.Column<bool>(type: "boolean", nullable: false),
                    need_no_need = table.Column<bool>(type: "boolean", nullable: false),
                    approval_lead_time = table.Column<bool>(type: "boolean", nullable: false),
                    trf_no = table.Column<bool>(type: "boolean", nullable: false),
                    target = table.Column<bool>(type: "boolean", nullable: false),
                    actual = table.Column<bool>(type: "boolean", nullable: false),
                    result = table.Column<bool>(type: "boolean", nullable: false),
                    result_email_date_to_supplier = table.Column<bool>(type: "boolean", nullable: false),
                    result_passed_failed = table.Column<bool>(type: "boolean", nullable: false),
                    remarks = table.Column<bool>(type: "boolean", nullable: false),
                    parts_shortage_date = table.Column<bool>(type: "boolean", nullable: false),
                    target_mold_usage_date = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_update_activity_matrix", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
       
            migrationBuilder.CreateTable(
                name: "UpdateActivities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    activity = table.Column<string>(type: "text", nullable: false),
                    actual = table.Column<bool>(type: "boolean", nullable: false),
                    approval_lead_time = table.Column<bool>(type: "boolean", nullable: false),
                    fabrication_lead_time = table.Column<bool>(type: "boolean", nullable: false),
                    limit_date = table.Column<bool>(type: "boolean", nullable: false),
                    need_no_need = table.Column<bool>(type: "boolean", nullable: false),
                    parts_shortage_date = table.Column<bool>(type: "boolean", nullable: false),
                    remaining_days_until_limit = table.Column<bool>(type: "boolean", nullable: false),
                    remarks = table.Column<bool>(type: "boolean", nullable: false),
                    result = table.Column<bool>(type: "boolean", nullable: false),
                    result_email_date_to_supplier = table.Column<bool>(type: "boolean", nullable: false),
                    result_passed_failed = table.Column<bool>(type: "boolean", nullable: false),
                    section = table.Column<string>(type: "text", nullable: false),
                    trf_no = table.Column<bool>(type: "boolean", nullable: false),
                    target = table.Column<bool>(type: "boolean", nullable: false),
                    target_mold_usage_date = table.Column<bool>(type: "boolean", nullable: false),
                    transfer_lead_time = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpdateActivities", x => x.id);
                });
        }
    }
}
