using LMS.Common.CQRS;
using LMS.Media.Core.Models;

namespace LMS.Media.Application.Queries.GetMediaByEntity;

public record GetMediaByEntityQuery(
    MediaEntityType EntityType,
    Guid EntityId) : IQuery;
