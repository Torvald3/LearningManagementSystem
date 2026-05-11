namespace LMS.Media.Application.Commands.ArchiveMedia;

public record ArchiveMediaResult(
    ArchiveMediaStatus Status,
    IEnumerable<string> Errors);
