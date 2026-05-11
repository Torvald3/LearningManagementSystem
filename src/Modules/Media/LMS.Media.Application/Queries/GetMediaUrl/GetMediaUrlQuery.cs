using LMS.Common.CQRS;

namespace LMS.Media.Application.Queries.GetMediaUrl;

public record GetMediaUrlQuery(Guid MediaId) : IQuery;
