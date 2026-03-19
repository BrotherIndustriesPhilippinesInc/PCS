using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class addImportedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "import_data",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    control_no = table.Column<string>(type: "text", nullable: false),
                    mother_moldcode = table.Column<string>(type: "text", nullable: false),
                    child_partcode = table.Column<string>(type: "text", nullable: false),
                    partname = table.Column<string>(type: "text", nullable: false),
                    model = table.Column<string>(type: "text", nullable: false),
                    supplier = table.Column<string>(type: "text", nullable: false),
                    mold_maker = table.Column<string>(type: "text", nullable: false),
                    supplier_mold_no = table.Column<string>(type: "text", nullable: false),
                    biph_mold_no = table.Column<string>(type: "text", nullable: false),
                    tooling_management = table.Column<string>(type: "text", nullable: true),
                    renewal_additional_mold = table.Column<string>(type: "text", nullable: false),
                    new_tooling_localization = table.Column<string>(type: "text", nullable: false),
                    transfer_tooling = table.Column<string>(type: "text", nullable: false),
                    change_material = table.Column<string>(type: "text", nullable: false),
                    new_model = table.Column<string>(type: "text", nullable: false),
                    non_concurrent = table.Column<string>(type: "text", nullable: false),
                    supplier_change_localization = table.Column<string>(type: "text", nullable: false),
                    other_4m = table.Column<string>(type: "text", nullable: false),
                    date_imported = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_import_data", x => x.id);
                });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        employee_id = table.Column<string>(type: "text", nullable: false),
            //        adid = table.Column<string>(type: "text", nullable: false),
            //        first_name = table.Column<string>(type: "text", nullable: false),
            //        last_name = table.Column<string>(type: "text", nullable: false),
            //        email = table.Column<string>(type: "text", nullable: false),
            //        section = table.Column<string>(type: "text", nullable: false),
            //        department = table.Column<string>(type: "text", nullable: false),
            //        authority = table.Column<string>(type: "text", nullable: false),
            //        created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.id);
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "import_data");

            //migrationBuilder.DropTable(
            //    name: "Users");
        }
    }
}
