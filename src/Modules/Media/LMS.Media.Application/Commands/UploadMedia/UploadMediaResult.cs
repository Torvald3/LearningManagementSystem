using LMS.Media.Application.Models;

namespace LMS.Media.Application.Commands.UploadMedia;

public record UploadMediaResult(
    UploadMediaStatus Status,
    MediaFile? MediaFile,
    IEnumerable<string> Errors);
