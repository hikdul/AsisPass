using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// migracion que crea los roles y el super usuario inicial
    /// </summary>
    public partial class rolesAndSA : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "390c8317-db17-4762-a895-083c4590d750", "SuperAdmin", "SUPERADMIN" },
                    { "16e33ec0-7fa5-4c84-b073-15ce21f4e60a", "dbcf99ef-40b7-43e0-9b19-7ab6c8ac5e46", "Admin", "ADMIN" },
                    { "073c51e1-fdab-4349-83d5-5b34cc82e541", "de3ec7c1-e1f5-45a4-99eb-98c217b0a58a", "Empresa", "EMPRESA" },
                    { "dddb7443-d5c8-4b38-ba6e-abd0ef20d9f3", "ded4258b-a253-4443-9053-22d6e4212b06", "Fiscal", "FISCAL" },
                    { "d95a2f3d-531f-4466-b6fe-2a69a6e49e5a", "318c6279-2c89-482e-8e25-80c5eed814ee", "Empleado", "EMPLEADO" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "71032b9d-207e-4c27-a0d6-b4fb94342ba8", 0, "089c80f0-1092-4d2d-bef0-264e51ae8e60", "desarrollo@automatismoslau.cl", true, false, null, "DESARROLLO@AUTOMATISMOSLAU.CL", "DESARROLLO@AUTOMATISMOSLAU.CL", "AQAAAAEAACcQAAAAECk/8OmgAk0044PAa7PvWN6TDXktpQaLSw/DfNHRbhtgi+RrpFR+RkcdviJK/yiowA==", "+56 9 3315 8879", true, "aa44e724-9c40-47d3-afaf-cbc7ed34e01c", false, "desarrollo@automatismoslau.cl" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "71032b9d-207e-4c27-a0d6-b4fb94342ba8" });
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "073c51e1-fdab-4349-83d5-5b34cc82e541");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "16e33ec0-7fa5-4c84-b073-15ce21f4e60a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d95a2f3d-531f-4466-b6fe-2a69a6e49e5a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dddb7443-d5c8-4b38-ba6e-abd0ef20d9f3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "71032b9d-207e-4c27-a0d6-b4fb94342ba8" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e2a8afe-fe62-4598-a773-1f89ce15af3a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "71032b9d-207e-4c27-a0d6-b4fb94342ba8");
        }
    }
}
