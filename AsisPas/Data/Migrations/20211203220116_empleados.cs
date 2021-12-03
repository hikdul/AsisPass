using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// empleados
    /// </summary>
    public partial class empleados : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AdmoEmpresas",
                table: "AdmoEmpresas");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "AdmoEmpresas",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdmoEmpresas",
                table: "AdmoEmpresas",
                column: "id");

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    act = table.Column<bool>(type: "bit", nullable: false),
                    userid = table.Column<int>(type: "int", nullable: false),
                    Empresaid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.id);
                    table.ForeignKey(
                        name: "FK_Empleados_Empresas_Empresaid",
                        column: x => x.Empresaid,
                        principalTable: "Empresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Empleados_Usuarios_userid",
                        column: x => x.userid,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmoEmpresas_Empresaid",
                table: "AdmoEmpresas",
                column: "Empresaid");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_Empresaid",
                table: "Empleados",
                column: "Empresaid");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_userid",
                table: "Empleados",
                column: "userid");
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdmoEmpresas",
                table: "AdmoEmpresas");

            migrationBuilder.DropIndex(
                name: "IX_AdmoEmpresas_Empresaid",
                table: "AdmoEmpresas");

            migrationBuilder.DropColumn(
                name: "id",
                table: "AdmoEmpresas");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdmoEmpresas",
                table: "AdmoEmpresas",
                columns: new[] { "Empresaid", "userid" });
        }
    }
}
