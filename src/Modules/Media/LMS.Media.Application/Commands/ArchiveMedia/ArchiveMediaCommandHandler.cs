using LMS.Common.CQRS;
using LMS.Media.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Media.Application.Commands.ArchiveMedia;

public class ArchiveMediaCommandHandler : ICommandHandler<ArchiveMediaCommand, ArchiveMediaResult>
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<ArchiveMediaCommandHandler> _logger;

    public ArchiveMediaCommandHandler(
        IMediaService mediaService,
        ILogger<ArchiveMediaCommandHandler> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    public async Task<ArchiveMediaResult> HandleAsync(
        ArchiveMediaCommand command,
        CancellationToken cancellationToken = default)
    {
        var archived = await _mediaService.ArchiveMediaAsync(
            command.MediaId,
            DateTime.UtcNow,
            cancellationToken);

        if (!archived)
        {
            _logger.LogWarning(
                "timestamp={Timestamp} level={Level} event={Event} media_id={MediaId}",
                DateTime.UtcNow,
                "WARN",
                "media.archive.not_found",
                command.MediaId);

            return new ArchiveMediaResult(
                ArchiveMediaStatus.NotFound,
                ["Media file not found."]);
        }

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} media_id={MediaId}",
            DateTime.UtcNow,
            "INFO",
            "media.archive.succeeded",
            command.MediaId);

        return new ArchiveMediaResult(
            ArchiveMediaStatus.Success,
            []);
    }
}
