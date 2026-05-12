using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Courses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "courses");

            migrationBuilder.CreateTable(
                name: "course",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Theme = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "course_member",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_course_member_course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "course_module",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_module", x => x.Id);
                    table.ForeignKey(
                        name: "FK_course_module_course_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lesson",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lesson", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lesson_course_module_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "courses",
                        principalTable: "course_module",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_member_CourseId_CourseOwner",
                schema: "courses",
                table: "course_member",
                column: "CourseId",
                unique: true,
                filter: "\"Role\" = 'CourseOwner'");

            migrationBuilder.CreateIndex(
                name: "IX_course_member_CourseId_Role",
                schema: "courses",
                table: "course_member",
                columns: new[] { "CourseId", "Role" });

            migrationBuilder.CreateIndex(
                name: "IX_course_member_CourseId_UserId",
                schema: "courses",
                table: "course_member",
                columns: new[] { "CourseId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_member_UserId",
                schema: "courses",
                table: "course_member",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_course_module_CourseId",
                schema: "courses",
                table: "course_module",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_course_module_CourseId_IsArchived",
                schema: "courses",
                table: "course_module",
                columns: new[] { "CourseId", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "IX_course_module_CourseId_IsArchived_Position",
                schema: "courses",
                table: "course_module",
                columns: new[] { "CourseId", "IsArchived", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_lesson_ModuleId",
                schema: "courses",
                table: "lesson",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_lesson_ModuleId_IsArchived",
                schema: "courses",
                table: "lesson",
                columns: new[] { "ModuleId", "IsArchived" });

            migrationBuilder.CreateIndex(
                name: "IX_lesson_ModuleId_IsArchived_Position",
                schema: "courses",
                table: "lesson",
                columns: new[] { "ModuleId", "IsArchived", "Position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_member",
                schema: "courses");

            migrationBuilder.DropTable(
                name: "lesson",
                schema: "courses");

            migrationBuilder.DropTable(
                name: "course_module",
                schema: "courses");

            migrationBuilder.DropTable(
                name: "course",
                schema: "courses");
        }
    }
}
