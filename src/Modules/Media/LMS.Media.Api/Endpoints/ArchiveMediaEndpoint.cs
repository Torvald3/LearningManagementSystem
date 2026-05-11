using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Media.Application.Commands.ArchiveMedia;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Media.Api.Endpoints;

public static class ArchiveMediaEndpoint
{
    public static RouteGroupBuilder MapArchiveMedia(this RouteGroupBuilder group)
    {
        group.MapDelete("/{mediaId:guid}", ArchiveMedia)
             .WithName(nameof(ArchiveMedia));

        return group;
    }

    private static async Task<IResult> ArchiveMedia(
        Guid mediaId,
        ICommandHandler<ArchiveMediaCommand> handler)
    {
        var result = await handler.HandleAsync(new ArchiveMediaCommand(mediaId));

        if (result.IsSuccess)
        {
            return Results.NoContent();
        }

        if (result.Error.Type == ErrorType.NotFound)
        {
            return Results.NotFound(result.Error.Message);
        }

        return Results.Problem("Unexpected error while archiving media.");
    }
}
