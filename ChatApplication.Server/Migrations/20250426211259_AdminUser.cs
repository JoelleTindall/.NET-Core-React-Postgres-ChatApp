using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class AdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AvatarId", "CreatedAt", "IsAdmin", "PasswordHash", "PasswordSalt", "UserName" },
                values: new object[] { 1, 1, new DateTime(2025, 4, 26, 21, 12, 58, 716, DateTimeKind.Utc).AddTicks(5568), true, "sZjib9QP5/l6jwBx0A4lmnOFYFBF87J5SRSDX2ciRqL4dJDCL2qQCmS+Cj0s3mLQgw+NmtvtuwO2xfIZyPKv0w==", "IiGE63gEmQfh/hpj5oodmAM5n/AcT3BDP9VRWfi3vroe4KLLIJ1FHMU9H4GD4HJ30sFYHs+NOxd+YDP9kG1jPzJJtyIruEpluPjBAK9nMZ8YHHXOaw9u6PX4NyzDZf6rQ8Jv/wzofjmOR8Jexm7fiGtVkVymsrh+VoO5K8lgxHo=", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
