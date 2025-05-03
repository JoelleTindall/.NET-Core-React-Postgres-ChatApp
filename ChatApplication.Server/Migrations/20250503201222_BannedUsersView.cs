using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApplication.Server.Migrations
{
    /// <inheritdoc />
    public partial class BannedUsersView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE VIEW BannedUsers AS
                SELECT "Id", "UserName", "IsBanned"
                FROM public."Users"
                where "Users"."IsBanned" = true
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP VIEW public.bannedusers;
                """
                );
        }
    }
}
