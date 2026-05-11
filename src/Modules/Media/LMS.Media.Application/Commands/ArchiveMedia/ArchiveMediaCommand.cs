using LMS.Common.CQRS;

namespace LMS.Media.Application.Commands.ArchiveMedia;

public record ArchiveMediaCommand(Guid MediaId) : ICommand;
