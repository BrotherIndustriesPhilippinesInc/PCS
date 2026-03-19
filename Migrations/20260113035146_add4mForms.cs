using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class add4mForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "4m_forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    submission_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    company_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    supplier_pic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    control_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    type_of_change = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    part_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    part_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    change_reason = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    pqc_pic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    attachment_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_4m_forms", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "4m_forms");
        }
    }
}
