using LMS.Common.CQRS;
using LMS.Media.Application.Models;
using LMS.Media.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Media.Application.Queries.GetMediaUrl;

public class GetMediaUrlQueryHandler : IQueryHandler<GetMediaUrlQuery, MediaReadUrl?>
{
    private readonly IMediaService _mediaService;
    private readonly IMediaStorage _mediaStorage;
    private readonly ILogger<GetMediaUrlQueryHandler> _logger;

    public GetMediaUrlQueryHandler(
        IMediaService mediaService,
        IMediaStorage mediaStorage,
        ILogger<GetMediaUrlQueryHandler> logger)
    {
        _mediaService = mediaService;
        _mediaStorage = mediaStorage;
        _logger = logger;
    }

    public async Task<MediaReadUrl?> Handle(
        GetMediaUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        var mediaFile = await _mediaService.GetMediaAsync(query.MediaId, cancellationToken);

        if (mediaFile is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} media_id={MediaId}",
                DateTime.UtcNow,
                "WARN",
                "media.url.not_found",
                query.MediaId);

            return null;
        }

        var mediaUrl = await _mediaStorage.GetReadUrlAsync(mediaFile.ObjectKey, cancellationToken);

        return new MediaReadUrl(
            mediaUrl.Url,
            mediaUrl.ExpiresAt);
    }
}
