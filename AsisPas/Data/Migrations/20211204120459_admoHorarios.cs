using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// admo horarios
    /// </summary>
    public partial class admoHorarios : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdmoHorarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    inicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Empleadoid = table.Column<int>(type: "int", nullable: false),
                    Horarioid = table.Column<int>(type: "int", nullable: false),
                    Admoid = table.Column<int>(type: "int", nullable: false),
                    Razon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmoHorarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_AdmoHorarios_AdmoEmpresas_Admoid",
                        column: x => x.Admoid,
                        principalTable: "AdmoEmpresas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdmoHorarios_Empleados_Empleadoid",
                        column: x => x.Empleadoid,
                        principalTable: "Empleados",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmoHorarios_Horarios_Horarioid",
                        column: x => x.Horarioid,
                        principalTable: "Horarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmoHorarios_Admoid",
                table: "AdmoHorarios",
                column: "Admoid");

            migrationBuilder.CreateIndex(
                name: "IX_AdmoHorarios_Empleadoid",
                table: "AdmoHorarios",
                column: "Empleadoid");

            migrationBuilder.CreateIndex(
                name: "IX_AdmoHorarios_Horarioid",
                table: "AdmoHorarios",
                column: "Horarioid");
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmoHorarios");
        }
    }
}
