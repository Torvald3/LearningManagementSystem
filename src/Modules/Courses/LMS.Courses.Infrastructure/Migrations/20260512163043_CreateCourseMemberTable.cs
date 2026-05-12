using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Courses.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateCourseMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE IF NOT EXISTS courses.course_member (
                    "Id" uuid NOT NULL,
                    "CourseId" uuid NOT NULL,
                    "UserId" uuid NOT NULL,
                    "Role" character varying(32) NOT NULL,
                    "CreatedAt" timestamp with time zone NOT NULL,
                    "UpdatedAt" timestamp with time zone NOT NULL,
                    CONSTRAINT "PK_course_member" PRIMARY KEY ("Id"),
                    CONSTRAINT "FK_course_member_course_CourseId" FOREIGN KEY ("CourseId")
                        REFERENCES courses.course ("Id") ON DELETE CASCADE
                );
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_course_member_CourseId"
                ON courses.course_member ("CourseId");
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_course_member_UserId"
                ON courses.course_member ("UserId");
                """);

            migrationBuilder.Sql("""
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_course_member_CourseId_UserId"
                ON courses.course_member ("CourseId", "UserId");
                """);

            migrationBuilder.Sql("""
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_course_member_CourseId_CourseOwner"
                ON courses.course_member ("CourseId")
                WHERE "Role" = 'CourseOwner';
                """);

            migrationBuilder.Sql("""
                CREATE INDEX IF NOT EXISTS "IX_course_member_CourseId_Role"
                ON courses.course_member ("CourseId", "Role");
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""DROP TABLE IF EXISTS courses.course_member;""");
        }
    }
}
