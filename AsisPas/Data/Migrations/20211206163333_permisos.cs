using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// permisos
    /// </summary>
    public partial class permisos : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    act = table.Column<bool>(type: "bit", nullable: false),
                    Empleadoid = table.Column<int>(type: "int", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PruebaFotografica = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AprobadoPorid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.id);
                    table.ForeignKey(
                        name: "FK_Permisos_AdmoEmpresas_AprobadoPorid",
                        column: x => x.AprobadoPorid,
                        principalTable: "AdmoEmpresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Permisos_Empleados_Empleadoid",
                        column: x => x.Empleadoid,
                        principalTable: "Empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_AprobadoPorid",
                table: "Permisos",
                column: "AprobadoPorid");

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Empleadoid",
                table: "Permisos",
                column: "Empleadoid");
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permisos");
        }
    }
}
