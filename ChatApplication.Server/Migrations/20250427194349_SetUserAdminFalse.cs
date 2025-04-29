using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class SetUserAdminFalse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 4, 27, 19, 43, 48, 869, DateTimeKind.Utc).AddTicks(8168), "6PIoyjwA5TSKVuYhKKBnInXmnsRFCLSBEsObl/HwK/r0QYzE6pXgqLL5OLNZ3JCw0aXThIDFG6GCh0B8jVVkEg==", "uODGNtsy8eSINKr2xruuVtla1WVNBWl7G2iLHj3Lv1qmqAM104yHCWqHfiHN5JZV2HoDEXVH/KPVlrFJz8u6GxIKJf7t87GB0PEN5/phZ3i++7TRRMz44zb4HPWSqG1avHwCm51ZcK4jGeykWzcOJIn5ltJK3zw7jOxa4M+fqOs=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash", "PasswordSalt" },
                values: new object[] { new DateTime(2025, 4, 26, 21, 49, 40, 936, DateTimeKind.Utc).AddTicks(8881), "tiJw8Jp8bLqnHKI9zQS/CycuH2lvQYSO4BF+9zmc/I7jh3TovbjCvsre/JiUwXbyc8m6qenC342ediotcN6NEw==", "8uzyQb1b8M+WnuOJcGpMvJ8xfzwMpyIyFIwj7gv57kL3oS40NC7mQqbAJ1YyDROEpHg7yacOqIcpzz4zjwD/6YcHlL53Mtb/SwrmIIc4boPHid8xz09bFyVvmxrWqU5qInR8oXLl9I2SQU66uV0qHE6h770iHGjY7vxLLCVMc8s=" });
        }
    }
}
