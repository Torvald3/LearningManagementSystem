using LMS.Common.CQRS;
using LMS.Users.Core.Services;

namespace LMS.Users.Application.Queries;

public class UserExistsQueryHandler : IQueryHandler<UserExistsQuery, bool>
{
    private readonly IUsersService _usersService;

    public UserExistsQueryHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public Task<bool> Handle(UserExistsQuery query, CancellationToken cancellationToken = default)
    {
        return _usersService.UserExistsAsync(query.UserId, cancellationToken);
    }
}