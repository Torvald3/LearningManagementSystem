using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Media.Application.Errors;
using LMS.Media.Application.Models;
using LMS.Media.Core.Configurations;
using LMS.Media.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Media.Application.Commands.UploadMedia;

public class UploadMediaCommandHandler : ICommandHandler<UploadMediaCommand, MediaFile>
{
    private readonly IMediaService _mediaService;
    private readonly IMediaStorage _mediaStorage;
    private readonly MediaStorageConfiguration _storageConfiguration;
    private readonly ILogger<UploadMediaCommandHandler> _logger;

    public UploadMediaCommandHandler(
        IMediaService mediaService,
        IMediaStorage mediaStorage,
        MediaStorageConfiguration storageConfiguration,
        ILogger<UploadMediaCommandHandler> logger)
    {
        _mediaService = mediaService;
        _mediaStorage = mediaStorage;
        _storageConfiguration = storageConfiguration;
        _logger = logger;
    }

    public async Task<Result<MediaFile>> HandleAsync(
        UploadMediaCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.EntityId == Guid.Empty)
        {
            return MediaErrors.EntityIdRequired;
        }

        if (command.Size <= 0)
        {
            return MediaErrors.FileEmpty;
        }

        var maxFileSizeMb = Math.Max(1, _storageConfiguration.MaxFileSizeMb);
        var maxFileSizeBytes = (long)maxFileSizeMb * 1024 * 1024;

        if (command.Size > maxFileSizeBytes)
        {
            return MediaErrors.FileTooLarge(maxFileSizeMb);
        }

        var now = DateTime.UtcNow;
        var mediaId = Guid.NewGuid();
        var safeFileName = SanitizeFileName(command.OriginalFileName);
        var objectKey = $"media/{command.EntityType.ToString().ToLowerInvariant()}/{command.EntityId:N}/{mediaId:N}/{safeFileName}";
        var contentType = string.IsNullOrWhiteSpace(command.ContentType)
            ? "application/octet-stream"
            : command.ContentType;

        var mediaFile = new LMS.Media.Core.Models.MediaFile
        {
            Id = mediaId,
            EntityType = command.EntityType,
            EntityId = command.EntityId,
            ObjectKey = objectKey,
            OriginalFileName = safeFileName,
            ContentType = contentType,
            Size = command.Size,
            CreatedAt = now,
            IsArchived = false
        };

        await _mediaStorage.UploadAsync(
            mediaFile.ObjectKey,
            command.Content,
            mediaFile.Size,
            mediaFile.ContentType,
            cancellationToken);

        await _mediaService.CreateMediaAsync(mediaFile, cancellationToken);

        _logger.LogInformation(
            "timestamp={Timestamp} level={Level} event={Event} media_id={MediaId} entity_type={EntityType} entity_id={EntityId}",
            DateTime.UtcNow,
            "INFO",
            "media.upload.succeeded",
            mediaFile.Id,
            mediaFile.EntityType,
            mediaFile.EntityId);

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

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName);

        if (string.IsNullOrWhiteSpace(name))
        {
            return "file";
        }

        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '-');
        }

        return name.Trim();
    }
}
