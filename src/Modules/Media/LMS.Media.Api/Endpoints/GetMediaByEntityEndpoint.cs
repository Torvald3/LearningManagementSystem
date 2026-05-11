using LMS.Common.CQRS;
using LMS.Media.Api.Models;
using LMS.Media.Application.Queries.GetMediaByEntity;
using LMS.Media.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MediaFileModel = LMS.Media.Application.Models.MediaFile;

namespace LMS.Media.Api.Endpoints;

public static class GetMediaByEntityEndpoint
{
    public static RouteGroupBuilder MapGetMediaByEntity(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetMediaByEntity)
             .WithName(nameof(GetMediaByEntity));

        return group;
    }

    private static async Task<IResult> GetMediaByEntity(
        string entityType,
        Guid entityId,
        IQueryHandler<GetMediaByEntityQuery, IReadOnlyList<MediaFileModel>> handler)
    {
        if (!Enum.TryParse<MediaEntityType>(entityType, ignoreCase: true, out var parsedEntityType))
        {
            IEnumerable<string> errors = ["EntityType is invalid."];
            return Results.BadRequest(errors);
        }

        if (entityId == Guid.Empty)
        {
            IEnumerable<string> errors = ["EntityId is required."];
            return Results.BadRequest(errors);
        }

        var mediaFiles = await handler.Handle(new GetMediaByEntityQuery(parsedEntityType, entityId));

        var response = mediaFiles
            .Select(mediaFile => new MediaResponse(
                mediaFile.Id,
                mediaFile.EntityType,
                mediaFile.EntityId,
                mediaFile.OriginalFileName,
                mediaFile.ContentType,
                mediaFile.Size,
                mediaFile.CreatedAt))
            .ToList();

        return Results.Ok(response);
    }
}
