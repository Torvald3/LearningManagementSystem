using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Users.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameAvatarUrlToAvatarMediaId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                schema: "users",
                table: "users");

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarMediaId",
                schema: "users",
                table: "users",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarMediaId",
                schema: "users",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                schema: "users",
                table: "users",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);
        }
    }
}
