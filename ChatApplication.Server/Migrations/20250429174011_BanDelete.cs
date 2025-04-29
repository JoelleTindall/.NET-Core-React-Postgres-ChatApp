using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class BanDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "Users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Chats",
                type: "boolean",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsBanned", "PasswordHash", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 4, 29, 17, 40, 10, 906, DateTimeKind.Utc).AddTicks(3823), false, "U1HV4LMfNmOZ2lH5+9z8ljWxn3x6wu/e8CzSQf7vpGO6YdIp+PESVQloQDkRrvLLsToNb7sDkYzBL+8WSZ4Clg==", "cOCr+pRnE00gryX9urWCDjoGjzKKg2YhzhnOQ3SyucKRZvhH0GLxJPz05jwYO6EtNIiHON6ftkI2jCRHcMNQBzj1flZXYkcH+vEmECeQ9D+gvD1RHvz9SVnughacZ/Z5byJh2+TEhZtFc0tzKAhq2D613C6/p0ymBtagYrxbAAE=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Chats");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 4, 27, 19, 43, 48, 869, DateTimeKind.Utc).AddTicks(8168), "6PIoyjwA5TSKVuYhKKBnInXmnsRFCLSBEsObl/HwK/r0QYzE6pXgqLL5OLNZ3JCw0aXThIDFG6GCh0B8jVVkEg==", "uODGNtsy8eSINKr2xruuVtla1WVNBWl7G2iLHj3Lv1qmqAM104yHCWqHfiHN5JZV2HoDEXVH/KPVlrFJz8u6GxIKJf7t87GB0PEN5/phZ3i++7TRRMz44zb4HPWSqG1avHwCm51ZcK4jGeykWzcOJIn5ltJK3zw7jOxa4M+fqOs=" });
        }
    }
}
