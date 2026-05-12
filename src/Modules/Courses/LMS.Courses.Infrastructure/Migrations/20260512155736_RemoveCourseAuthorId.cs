using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Courses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCourseAuthorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                schema: "courses",
                table: "course");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                schema: "courses",
                table: "course",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
