using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// mapeo de sedes
    /// </summary>
    public partial class Sedes : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sedes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Empresaid = table.Column<int>(type: "int", nullable: false),
                    lat = table.Column<double>(type: "float", nullable: false),
                    lng = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.id);
                    table.ForeignKey(
                        name: "FK_Sedes_Empresas_Empresaid",
                        column: x => x.Empresaid,
                        principalTable: "Empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_Empresaid",
                table: "Sedes",
                column: "Empresaid");
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sedes");
        }
    }
}
