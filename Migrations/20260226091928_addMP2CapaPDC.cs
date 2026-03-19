using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class addMP2CapaPDC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mp2_capa_pdc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ControlNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PartsShortageDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TargetMoldUsageDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InputBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mp2_capa_pdc", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mp2_capa_pdc");
        }
    }
}
