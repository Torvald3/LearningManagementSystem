using LMS.Common.CQRS;
using LMS.Media.Api.Models;
using LMS.Media.Application.Models;
using LMS.Media.Application.Queries.GetMediaUrl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace LMS.Media.Api.Endpoints;

public static class GetMediaUrlEndpoint
{
    public static RouteGroupBuilder MapGetMediaUrl(this RouteGroupBuilder group)
    {
        group.MapGet("/{mediaId:guid}/url", GetMediaUrl)
             .WithName(nameof(GetMediaUrl));

        return group;
    }

    private static async Task<IResult> GetMediaUrl(
        Guid mediaId,
        IQueryHandler<GetMediaUrlQuery, MediaReadUrl> handler)
    {
        var result = await handler.Handle(new GetMediaUrlQuery(mediaId));

        if (result.IsFailure)
        {
            return Results.NotFound();
        }

        var mediaUrl = result.Value;

        return Results.Ok(new MediaUrlResponse(
            mediaUrl.Url,
            mediaUrl.ExpiresAt));
    }
}
