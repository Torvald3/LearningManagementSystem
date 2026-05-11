using LMS.Common.Results;
using Microsoft.AspNetCore.Http;

namespace LMS.Courses.Api.Endpoints;

internal static class CourseEndpointResults
{
    public static IResult FromError(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(new[] { error.Message }),
            ErrorType.NotFound => Results.NotFound(new[] { error.Message }),
            ErrorType.Conflict => Results.Conflict(error.Message),
            _ => Results.Problem(error.Message)
        };
    }
}
