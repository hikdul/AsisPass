using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// para la creacion de empresas
    /// </summary>
    public partial class Initial : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rut = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Rubro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.id);
                });
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
