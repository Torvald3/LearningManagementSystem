using LMS.Common.CQRS;
using LMS.Media.Api.Models;
using LMS.Media.Application.Commands.UploadMedia;
using LMS.Media.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Media.Api.Endpoints;

public static class UploadMediaEndpoint
{
    public static RouteGroupBuilder MapUploadMedia(this RouteGroupBuilder group)
    {
        group.MapPost("/", UploadMedia)
             .WithName(nameof(UploadMedia))
             .Accepts<IFormFile>("multipart/form-data");

        return group;
    }

    private static async Task<IResult> UploadMedia(
        HttpRequest request,
        ICommandHandler<UploadMediaCommand, UploadMediaResult> handler,
        CancellationToken cancellationToken)
    {
        if (!request.HasFormContentType)
        {
            IEnumerable<string> errors = ["Request must be multipart/form-data."];
            return Results.BadRequest(errors);
        }

        var form = await request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("file");

        if (file is null)
        {
            IEnumerable<string> errors = ["File is required."];
            return Results.BadRequest(errors);
        }

        if (!Enum.TryParse<MediaEntityType>(form["entityType"].ToString(), ignoreCase: true, out var entityType))
        {
            IEnumerable<string> errors = ["EntityType is invalid."];
            return Results.BadRequest(errors);
        }

        if (!Guid.TryParse(form["entityId"].ToString(), out var entityId))
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

        return result.Status switch
        {
            UploadMediaStatus.InvalidEntityId => Results.BadRequest(result.Errors),
            UploadMediaStatus.EmptyFile => Results.BadRequest(result.Errors),
            UploadMediaStatus.FileTooLarge => Results.BadRequest(result.Errors),
            UploadMediaStatus.Success => Results.Created(
                $"/api/media/{result.MediaFile!.Id}",
                new MediaResponse(
                    result.MediaFile.Id,
                    result.MediaFile.EntityType,
                    result.MediaFile.EntityId,
                    result.MediaFile.OriginalFileName,
                    result.MediaFile.ContentType,
                    result.MediaFile.Size,
                    result.MediaFile.CreatedAt)),
            _ => Results.Problem("Unexpected error while uploading media.")
        };
    }
}
