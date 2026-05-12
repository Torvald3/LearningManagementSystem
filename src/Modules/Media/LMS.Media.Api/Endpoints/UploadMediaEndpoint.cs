using LMS.Common.CQRS;
using LMS.Media.Api.Models;
using LMS.Media.Application.Commands.UploadMedia;
using LMS.Media.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MediaFileModel = LMS.Media.Application.Models.MediaFile;

namespace LMS.Media.Api.Endpoints;

public static class UploadMediaEndpoint
{
    public static RouteGroupBuilder MapUploadMedia(this RouteGroupBuilder group)
    {
        group.MapPost("/", UploadMedia)
             .WithName(nameof(UploadMedia))
             .Accepts<IFormFile>("multipart/form-data")
             .DisableAntiforgery();

        return group;
    }

    private static async Task<IResult> UploadMedia(
        [FromForm] IFormFile? file,
        [FromForm] MediaEntityType entityType,
        [FromForm] Guid entityId,
        ICommandHandler<UploadMediaCommand, MediaFileModel> handler,
        CancellationToken cancellationToken)
    {
        if (file is null)
        {
            IEnumerable<string> errors = ["File is required."];
            return Results.BadRequest(errors);
        }

        if (!Enum.IsDefined(entityType))
        {
            IEnumerable<string> errors = ["EntityType is invalid."];
            return Results.BadRequest(errors);
        }

        if (entityId == Guid.Empty)
        {
            IEnumerable<string> errors = ["EntityId is invalid."];
            return Results.BadRequest(errors);
        }

        await using var stream = file.OpenReadStream();

        var result = await handler.HandleAsync(
            new UploadMediaCommand(
                entityType,
                entityId,
                file.FileName,
                file.ContentType,
                file.Length,
                stream),
            cancellationToken);

        if (result.IsFailure)
        {
            IEnumerable<string> errors = [result.Error.Message];
            return Results.BadRequest(errors);
        }

        var mediaFile = result.Value;

        return Results.Created(
            $"/api/media/{mediaFile.Id}",
            new MediaResponse(
                mediaFile.Id,
                mediaFile.EntityType,
                mediaFile.EntityId,
                mediaFile.OriginalFileName,
                mediaFile.ContentType,
                mediaFile.Size,
                mediaFile.CreatedAt));
    }
}
