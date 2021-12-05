using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// registro de marcas
    /// </summary>
    public partial class marcas : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Marcaciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    marca = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sedeid = table.Column<int>(type: "int", nullable: false),
                    Gate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Empleadoid = table.Column<int>(type: "int", nullable: false),
                    Horarioid = table.Column<int>(type: "int", nullable: false),
                    TipoIngreso = table.Column<int>(type: "int", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    key = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcaciones", x => x.id);
                    table.ForeignKey(
                        name: "FK_Marcaciones_Empleados_Empleadoid",
                        column: x => x.Empleadoid,
                        principalTable: "Empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Marcaciones_Horarios_Horarioid",
                        column: x => x.Horarioid,
                        principalTable: "Horarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Marcaciones_Sedes_Sedeid",
                        column: x => x.Sedeid,
                        principalTable: "Sedes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Marcaciones_Empleadoid",
                table: "Marcaciones",
                column: "Empleadoid");

            migrationBuilder.CreateIndex(
                name: "IX_Marcaciones_Horarioid",
                table: "Marcaciones",
                column: "Horarioid");

            migrationBuilder.CreateIndex(
                name: "IX_Marcaciones_Sedeid",
                table: "Marcaciones",
                column: "Sedeid");
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Marcaciones");
        }
    }
}
