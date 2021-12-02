using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// para agregar el horario
    /// </summary>
    public partial class Horario : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    hi = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    hf = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    hbi = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    hbf = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    diaSiguiente = table.Column<bool>(type: "bit", nullable: false),
                    sinDescanzo = table.Column<bool>(type: "bit", nullable: false),
                    act = table.Column<bool>(type: "bit", nullable: false),
                    Empresaid = table.Column<int>(type: "int", nullable: false),
                    Domingo = table.Column<bool>(type: "bit", nullable: false),
                    Lunes = table.Column<bool>(type: "bit", nullable: false),
                    Martes = table.Column<bool>(type: "bit", nullable: false),
                    Miercoles = table.Column<bool>(type: "bit", nullable: false),
                    Jueves = table.Column<bool>(type: "bit", nullable: false),
                    Viernes = table.Column<bool>(type: "bit", nullable: false),
                    Sabado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_Horarios_Empresas_Empresaid",
                        column: x => x.Empresaid,
                        principalTable: "Empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Horarios_Empresaid",
                table: "Horarios",
                column: "Empresaid");
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Horarios");
        }
    }
}
