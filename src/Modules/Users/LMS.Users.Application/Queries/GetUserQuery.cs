using LMS.Common.CQRS;

namespace LMS.Users.Application.Queries;

public record GetUserQuery(Guid UserId) : IQuery;
