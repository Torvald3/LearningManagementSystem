using LMS.Media.Core.Models;
using LMS.Media.Core.Services;
using LMS.Media.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace LMS.Media.Infrastructure.Implementation;

public class MediaService : IMediaService
{
    private readonly MediaDbContext _dbContext;

    public MediaService(MediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateMediaAsync(MediaFile mediaFile, CancellationToken cancellationToken = default)
    {
        _dbContext.MediaFiles.Add(new()
        {
            Id = mediaFile.Id,
            EntityType = mediaFile.EntityType,
            EntityId = mediaFile.EntityId,
            ObjectKey = mediaFile.ObjectKey,
            OriginalFileName = mediaFile.OriginalFileName,
            ContentType = mediaFile.ContentType,
            Size = mediaFile.Size,
            CreatedAt = mediaFile.CreatedAt,
            IsArchived = mediaFile.IsArchived,
            ArchivedAt = mediaFile.ArchivedAt
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<MediaFile?> GetMediaAsync(Guid mediaId, CancellationToken cancellationToken = default)
    {
        var mediaFile = await _dbContext.MediaFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == mediaId && !x.IsArchived, cancellationToken);

        if (mediaFile is null)
        {
            return null;
        }

        return new MediaFile
        {
            Id = mediaFile.Id,
            EntityType = mediaFile.EntityType,
            EntityId = mediaFile.EntityId,
            ObjectKey = mediaFile.ObjectKey,
            OriginalFileName = mediaFile.OriginalFileName,
            ContentType = mediaFile.ContentType,
            Size = mediaFile.Size,
            CreatedAt = mediaFile.CreatedAt,
            IsArchived = mediaFile.IsArchived,
            ArchivedAt = mediaFile.ArchivedAt
        };
    }

    public async Task<IReadOnlyList<MediaFile>> GetMediaByEntityAsync(
        MediaEntityType entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.MediaFiles
            .AsNoTracking()
            .Where(x => x.EntityType == entityType && x.EntityId == entityId && !x.IsArchived)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new MediaFile
            {
                Id = x.Id,
                EntityType = x.EntityType,
                EntityId = x.EntityId,
                ObjectKey = x.ObjectKey,
                OriginalFileName = x.OriginalFileName,
                ContentType = x.ContentType,
                Size = x.Size,
                CreatedAt = x.CreatedAt,
                IsArchived = x.IsArchived,
                ArchivedAt = x.ArchivedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ArchiveMediaAsync(
        Guid mediaId,
        DateTime archivedAt,
        CancellationToken cancellationToken = default)
    {
        var mediaFile = await _dbContext.MediaFiles
            .FirstOrDefaultAsync(x => x.Id == mediaId && !x.IsArchived, cancellationToken);

        if (mediaFile is null)
        {
            return false;
        }

        mediaFile.IsArchived = true;
        mediaFile.ArchivedAt = archivedAt;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
