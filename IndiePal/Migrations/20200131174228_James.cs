using Microsoft.EntityFrameworkCore.Migrations;

namespace IndiePal.Migrations
{
    public partial class James : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "00000000-ffff-ffff-ffff-ffffffffffff", 0, "2b5f2190-6f8e-4265-80a0-cfbb47be4a86", "admin@admin.com", true, "admin", "admin", false, null, "ADMIN@ADMIN.COM", "ADMIN@ADMIN.COM", "AQAAAAEAACcQAAAAEDx9974kLLO9MRPUbO+BVTn2QevWKJn1Dvl+2ve0Y7BH5zO97ZY9f0Ik2m+cMdygZg==", null, false, "7f434309-a4d9-48e9-9ebb-8803db794577", false, "adminguy" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff");
        }
    }
}
