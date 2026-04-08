using LMS.Courses.Api.Models;
using LMS.Courses.Core.Models;
using LMS.Courses.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LMS.Common.Observability.Metrics;

namespace LMS.Courses.Api.Endpoints;

public static class CourseEndpoints
{
    public static IEndpointRouteBuilder MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/courses").WithTags("Courses");

        group.MapPost("/", async (
            CreateCourseRequest request,
            CoursesDbContext dbContext,
            AppMetrics metrics,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("Courses.CreateCourse");
            var timestamp = DateTime.UtcNow;

            logger.LogInformation(
                "timestamp={Timestamp} level={Level} event={Event} title_length={TitleLength} theme_length={ThemeLength}",
                timestamp,
                "INFO",
                "course.create.requested",
                request.Title?.Length ?? 0,
                request.Theme?.Length ?? 0);

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                logger.LogWarning(
                    "timestamp={Timestamp} level={Level} event={Event}",
                    DateTime.UtcNow,
                    "WARN",
                    "course.create.validation_failed.title");

                return Results.BadRequest("Title is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Theme))
            {
                logger.LogWarning(
                    "timestamp={Timestamp} level={Level} event={Event}",
                    DateTime.UtcNow,
                    "WARN",
                    "course.create.validation_failed.theme");

                return Results.BadRequest("Theme is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                logger.LogWarning(
                    "timestamp={Timestamp} level={Level} event={Event}",
                    DateTime.UtcNow,
                    "WARN",
                    "course.create.validation_failed.description");

                return Results.BadRequest("Description is required.");
            }

            try
            {
                var now = DateTime.UtcNow;

                var course = new Course
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title.Trim(),
                    Theme = request.Theme.Trim(),
                    Description = request.Description.Trim(),
                    CreatedAt = now,
                    UpdatedAt = now
                };

                dbContext.Courses.Add(course);
                await dbContext.SaveChangesAsync();

                metrics.CourseCreated(course.Id);

                logger.LogInformation(
                    "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                    DateTime.UtcNow,
                    "INFO",
                    "course.create.succeeded",
                    course.Id);

                var response = new CourseResponse
                {
                    Id = course.Id,
                    Title = course.Title,
                    Theme = course.Theme,
                    Description = course.Description,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt
                };

                return Results.Created($"/api/courses/{course.Id}", response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "timestamp={Timestamp} level={Level} event={Event}",
                    DateTime.UtcNow,
                    "ERROR",
                    "course.create.failed");

                return Results.Problem("Unexpected error while creating course.");
            }
        });

        group.MapPut("/{id:guid}/description", async (
            Guid id,
            UpdateCourseDescriptionRequest request,
            CoursesDbContext dbContext,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("Courses.UpdateDescription");

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                logger.LogWarning(
                    "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                    DateTime.UtcNow,
                    "WARN",
                    "course.update_description.validation_failed",
                    id);

                return Results.BadRequest("Description is required.");
            }

            try
            {
                var course = await dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id);

                if (course is null)
                {
                    logger.LogWarning(
                        "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                        DateTime.UtcNow,
                        "WARN",
                        "course.update_description.not_found",
                        id);

                    return Results.NotFound();
                }

                course.Description = request.Description.Trim();
                course.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                logger.LogInformation(
                    "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                    DateTime.UtcNow,
                    "INFO",
                    "course.update_description.succeeded",
                    id);

                var response = new CourseResponse
                {
                    Id = course.Id,
                    Title = course.Title,
                    Theme = course.Theme,
                    Description = course.Description,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt
                };

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                    DateTime.UtcNow,
                    "ERROR",
                    "course.update_description.failed",
                    id);

                return Results.Problem("Unexpected error while updating course description.");
            }
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            CoursesDbContext dbContext,
            AppMetrics metrics,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("Courses.DeleteCourse");

            try
            {
                var course = await dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id);

                if (course is null)
                {
                    logger.LogWarning(
                        "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                        DateTime.UtcNow,
                        "WARN",
                        "course.delete.not_found",
                        id);

                    return Results.NotFound();
                }

                dbContext.Courses.Remove(course);
                await dbContext.SaveChangesAsync();

                metrics.CourseDeleted(id);

                logger.LogInformation(
                    "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                    DateTime.UtcNow,
                    "INFO",
                    "course.delete.succeeded",
                    id);

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                    DateTime.UtcNow,
                    "ERROR",
                    "course.delete.failed",
                    id);

                return Results.Problem("Unexpected error while deleting course.");
            }
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            CoursesDbContext dbContext,
            ILoggerFactory loggerFactory) =>
        {
            var logger = loggerFactory.CreateLogger("Courses.GetCourse");

            try
            {
                var course = await dbContext.Courses.FirstOrDefaultAsync(x => x.Id == id);

                if (course is null)
                {
                    logger.LogWarning(
                        "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                        DateTime.UtcNow,
                        "WARN",
                        "course.get.not_found",
                        id);

                    return Results.NotFound();
                }

                logger.LogInformation(
                    "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                    DateTime.UtcNow,
                    "INFO",
                    "course.get.succeeded",
                    id);

                var response = new CourseResponse
                {
                    Id = course.Id,
                    Title = course.Title,
                    Theme = course.Theme,
                    Description = course.Description,
                    CreatedAt = course.CreatedAt,
                    UpdatedAt = course.UpdatedAt
                };

                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "timestamp={Timestamp} level={Level} event={Event} course_id={CourseId}",
                    DateTime.UtcNow,
                    "ERROR",
                    "course.get.failed",
                    id);

                return Results.Problem("Unexpected error while getting course.");
            }
        });

        return app;
    }
}