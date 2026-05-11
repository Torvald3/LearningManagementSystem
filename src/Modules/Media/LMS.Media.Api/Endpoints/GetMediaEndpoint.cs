using LMS.Common.CQRS;
using LMS.Media.Api.Models;
using LMS.Media.Application.Models;
using LMS.Media.Application.Queries.GetMedia;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Media.Api.Endpoints;

public static class GetMediaEndpoint
{
    public static RouteGroupBuilder MapGetMedia(this RouteGroupBuilder group)
    {
        group.MapGet("/{mediaId:guid}", GetMedia)
             .WithName(nameof(GetMedia));

        return group;
    }

    private static async Task<IResult> GetMedia(
        Guid mediaId,
        IQueryHandler<GetMediaQuery, MediaFile?> handler)
    {
        var mediaFile = await handler.Handle(new GetMediaQuery(mediaId));

        if (mediaFile is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new MediaResponse(
            mediaFile.Id,
            mediaFile.EntityType,
            mediaFile.EntityId,
            mediaFile.OriginalFileName,
            mediaFile.ContentType,
            mediaFile.Size,
            mediaFile.CreatedAt));
    }
}
