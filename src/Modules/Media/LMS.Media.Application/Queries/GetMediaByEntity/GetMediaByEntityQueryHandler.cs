using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Media.Application.Errors;
using LMS.Media.Application.Models;
using LMS.Media.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Media.Application.Queries.GetMediaByEntity;

public class GetMediaByEntityQueryHandler : IQueryHandler<GetMediaByEntityQuery, List<MediaFile>>
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<GetMediaByEntityQueryHandler> _logger;

    public GetMediaByEntityQueryHandler(
        IMediaService mediaService,
        ILogger<GetMediaByEntityQueryHandler> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<Result<List<MediaFile>>> Handle(
        GetMediaByEntityQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.EntityId == Guid.Empty)
        {
            return MediaErrors.EntityIdRequired;
        }

        var mediaFiles = await _mediaService.GetMediaByEntityAsync(
            query.EntityType,
            query.EntityId,
            cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} entity_type={EntityType} entity_id={EntityId} media_count={MediaCount}",
            DateTime.UtcNow,
            "INFO",
            "media.get_by_entity.succeeded",
            query.EntityType,
            query.EntityId,
            mediaFiles.Count);

        List<MediaFile> result = mediaFiles
            .Select(mediaFile => new MediaFile(
                mediaFile.Id,
                mediaFile.EntityType,
                mediaFile.EntityId,
                mediaFile.ObjectKey,
                mediaFile.OriginalFileName,
                mediaFile.ContentType,
                mediaFile.Size,
                mediaFile.CreatedAt))
            .ToList();

        return result;
    }
}
