using LMS.Common.CQRS;

namespace LMS.Media.Application.Queries.GetMedia;

public record GetMediaQuery(Guid MediaId) : IQuery;
