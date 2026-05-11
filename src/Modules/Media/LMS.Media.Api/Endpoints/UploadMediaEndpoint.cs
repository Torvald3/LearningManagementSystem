using LMS.Common.CQRS;
using LMS.Media.Api.Models;
using LMS.Media.Application.Commands.UploadMedia;
using LMS.Media.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MediaFileModel = LMS.Media.Application.Models.MediaFile;

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
        ICommandHandler<UploadMediaCommand, MediaFileModel> handler,
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
