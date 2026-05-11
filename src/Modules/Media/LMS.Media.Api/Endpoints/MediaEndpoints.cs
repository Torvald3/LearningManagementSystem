using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Media.Api.Endpoints;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMediaEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/media")
            .WithTags("Media");

        group.MapUploadMedia()
             .MapGetMediaByEntity()
             .MapGetMedia()
             .MapGetMediaUrl()
             .MapArchiveMedia();

        return group;
    }
}
