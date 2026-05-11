using LMS.Common.CQRS;

namespace LMS.Users.Application.Queries;

public record UserExistsQuery(Guid UserId) : IQuery;