using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityCurrentProcess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "toolingQuotationRequestApprovals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "toolingQuotationRequestApprovals",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        actual_approval_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        control_no = table.Column<string>(type: "text", nullable: false),
            //        limit_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            //        remarks = table.Column<string>(type: "text", nullable: true),
            //        target_approval_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_toolingQuotationRequestApprovals", x => x.id);
            //    });
        }
    }
}
