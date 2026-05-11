using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Media.Application.Errors;
using LMS.Media.Application.Models;
using LMS.Media.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Media.Application.Queries.GetMedia;

public class GetMediaQueryHandler : IQueryHandler<GetMediaQuery, MediaFile>
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<GetMediaQueryHandler> _logger;

    public GetMediaQueryHandler(
        IMediaService mediaService,
        ILogger<GetMediaQueryHandler> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<Result<MediaFile>> Handle(GetMediaQuery query, CancellationToken cancellationToken = default)
    {
        var mediaFile = await _mediaService.GetMediaAsync(query.MediaId, cancellationToken);

        if (mediaFile is null)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} media_id={MediaId}",
                DateTime.UtcNow,
                "WARN",
                "media.get.not_found",
                query.MediaId);

            return MediaErrors.NotFound(query.MediaId);
        }

        return new MediaFile(
            mediaFile.Id,
            mediaFile.EntityType,
            mediaFile.EntityId,
            mediaFile.ObjectKey,
            mediaFile.OriginalFileName,
            mediaFile.ContentType,
            mediaFile.Size,
            mediaFile.CreatedAt);
    }
}
