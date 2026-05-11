using LMS.Common.CQRS;
using LMS.Media.Api.Models;
using LMS.Media.Application.Queries.GetMedia;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MediaFileModel = LMS.Media.Application.Models.MediaFile;

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
        IQueryHandler<GetMediaQuery, MediaFileModel> handler)
    {
        var result = await handler.Handle(new GetMediaQuery(mediaId));

        if (result.IsFailure)
        {
            return Results.NotFound();
        }

        var mediaFile = result.Value;

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
