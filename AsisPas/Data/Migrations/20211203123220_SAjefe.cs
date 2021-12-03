using Microsoft.EntityFrameworkCore.Migrations;

namespace AsisPas.Data.Migrations
{
    /// <summary>
    /// ...
    /// </summary>
    public partial class SAjefe : Migration
    {
        /// <summary>
        /// up
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "71032b9d-207e-4c27-a0d6-b4fb94342ba8" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "71032b9d-207e-4c27-a0d6-b4fb94342ba8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "073c51e1-fdab-4349-83d5-5b34cc82e541",
                column: "ConcurrencyStamp",
                value: "5813d813-a0b3-439a-bdc0-bd616cf5607f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "16e33ec0-7fa5-4c84-b073-15ce21f4e60a",
                column: "ConcurrencyStamp",
                value: "a24f757c-f248-40fe-bbf2-60b51af0955d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e2a8afe-fe62-4598-a773-1f89ce15af3a",
                column: "ConcurrencyStamp",
                value: "19343207-27ec-43ca-8cc5-540a5cb14d66");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d95a2f3d-531f-4466-b6fe-2a69a6e49e5a",
                column: "ConcurrencyStamp",
                value: "5f5a76e5-36b9-4490-a4fd-4803e15ec3dd");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dddb7443-d5c8-4b38-ba6e-abd0ef20d9f3",
                column: "ConcurrencyStamp",
                value: "eb8b98d0-29df-4165-af0d-90db1f8a8134");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "d6884458-12f4-492e-baa6-c996a8ecb32d", 0, "73732409-af00-4b0c-9ef3-eee11fe7c029", "desarrollo@automatismoslau.cl", true, false, null, "DESARROLLO@AUTOMATISMOSLAU.CL", "DESARROLLO@AUTOMATISMOSLAU.CL", "AQAAAAEAACcQAAAAECMXLqa3KW55Cdbb+6Em2CTR/orGZTd8Q9V96hEWMj3eDnJU5bQviQFstkxk7nYJZw==", "+56 9 3315 8879", true, "ef7c2a61-1b12-4383-9ca1-885514e75b8a", false, "desarrollo@automatismoslau.cl" },
                    { "2c66b1a5-11fe-4aaf-8716-af0f50772a5e", 0, "f4e2e853-8fea-4f5f-8813-be5f25de03cd", "pcortes@automatismoslau.cl", true, false, null, "PCORTES@AUTOMATISMOSLAU.CL", "PCORTES@AUTOMATISMOSLAU.CL", "AQAAAAEAACcQAAAAEIiYkBCgLxw3bRxqS+i2DSU6VZbMgHuj8GXGapxcZ96hmLtuKpPoAr67mqaQn86XFA==", "+56 9 9499 8131", true, "a1e56a33-7389-4548-abd8-ae40c39adfe1", false, "pcortes@automatismoslau.cl" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "d6884458-12f4-492e-baa6-c996a8ecb32d" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "2c66b1a5-11fe-4aaf-8716-af0f50772a5e" });
        }
        /// <summary>
        /// down
        /// </summary>
        /// <param name="migrationBuilder"></param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "2c66b1a5-11fe-4aaf-8716-af0f50772a5e" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "d6884458-12f4-492e-baa6-c996a8ecb32d" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2c66b1a5-11fe-4aaf-8716-af0f50772a5e");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d6884458-12f4-492e-baa6-c996a8ecb32d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "073c51e1-fdab-4349-83d5-5b34cc82e541",
                column: "ConcurrencyStamp",
                value: "de3ec7c1-e1f5-45a4-99eb-98c217b0a58a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "16e33ec0-7fa5-4c84-b073-15ce21f4e60a",
                column: "ConcurrencyStamp",
                value: "dbcf99ef-40b7-43e0-9b19-7ab6c8ac5e46");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e2a8afe-fe62-4598-a773-1f89ce15af3a",
                column: "ConcurrencyStamp",
                value: "390c8317-db17-4762-a895-083c4590d750");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d95a2f3d-531f-4466-b6fe-2a69a6e49e5a",
                column: "ConcurrencyStamp",
                value: "318c6279-2c89-482e-8e25-80c5eed814ee");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dddb7443-d5c8-4b38-ba6e-abd0ef20d9f3",
                column: "ConcurrencyStamp",
                value: "ded4258b-a253-4443-9053-22d6e4212b06");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "71032b9d-207e-4c27-a0d6-b4fb94342ba8", 0, "089c80f0-1092-4d2d-bef0-264e51ae8e60", "desarrollo@automatismoslau.cl", true, false, null, "DESARROLLO@AUTOMATISMOSLAU.CL", "DESARROLLO@AUTOMATISMOSLAU.CL", "AQAAAAEAACcQAAAAECk/8OmgAk0044PAa7PvWN6TDXktpQaLSw/DfNHRbhtgi+RrpFR+RkcdviJK/yiowA==", "+56 9 3315 8879", true, "aa44e724-9c40-47d3-afaf-cbc7ed34e01c", false, "desarrollo@automatismoslau.cl" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "5e2a8afe-fe62-4598-a773-1f89ce15af3a", "71032b9d-207e-4c27-a0d6-b4fb94342ba8" });
        }
    }
}
