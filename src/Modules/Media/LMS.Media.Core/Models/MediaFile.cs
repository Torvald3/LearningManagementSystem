namespace LMS.Media.Core.Models;

public class MediaFile
{
    public Guid Id { get; set; }

    public MediaEntityType EntityType { get; set; }

    public Guid EntityId { get; set; }

    public string ObjectKey { get; set; } = string.Empty;

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long Size { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsArchived { get; set; }

    public DateTime? ArchivedAt { get; set; }
}
