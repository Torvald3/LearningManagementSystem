using LMS.Common.CQRS;
using LMS.Media.Core.Models;

namespace LMS.Media.Application.Commands.UploadMedia;

public record UploadMediaCommand(
    MediaEntityType EntityType,
    Guid EntityId,
    string OriginalFileName,
    string ContentType,
    long Size,
    Stream Content) : ICommand;
