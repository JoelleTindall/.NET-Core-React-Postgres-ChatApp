using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class AdminDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 4, 26, 21, 49, 40, 936, DateTimeKind.Utc).AddTicks(8881), "tiJw8Jp8bLqnHKI9zQS/CycuH2lvQYSO4BF+9zmc/I7jh3TovbjCvsre/JiUwXbyc8m6qenC342ediotcN6NEw==", "8uzyQb1b8M+WnuOJcGpMvJ8xfzwMpyIyFIwj7gv57kL3oS40NC7mQqbAJ1YyDROEpHg7yacOqIcpzz4zjwD/6YcHlL53Mtb/SwrmIIc4boPHid8xz09bFyVvmxrWqU5qInR8oXLl9I2SQU66uV0qHE6h770iHGjY7vxLLCVMc8s=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 4, 26, 21, 12, 58, 716, DateTimeKind.Utc).AddTicks(5568), "sZjib9QP5/l6jwBx0A4lmnOFYFBF87J5SRSDX2ciRqL4dJDCL2qQCmS+Cj0s3mLQgw+NmtvtuwO2xfIZyPKv0w==", "IiGE63gEmQfh/hpj5oodmAM5n/AcT3BDP9VRWfi3vroe4KLLIJ1FHMU9H4GD4HJ30sFYHs+NOxd+YDP9kG1jPzJJtyIruEpluPjBAK9nMZ8YHHXOaw9u6PX4NyzDZf6rQ8Jv/wzofjmOR8Jexm7fiGtVkVymsrh+VoO5K8lgxHo=" });
        }
    }
}
