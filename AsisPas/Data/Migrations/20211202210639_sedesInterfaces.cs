using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// /
    /// </summary>
    public partial class sedesInterfaces : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "act",
                table: "Sedes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "act",
                table: "Sedes");
        }
    }
}
