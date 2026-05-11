using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Users.Core.Services;

namespace LMS.Users.Application.Queries;

public class UserExistsQueryHandler : IQueryHandler<UserExistsQuery, bool>
{
    private readonly IUsersService _usersService;

    public UserExistsQueryHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<Result<bool>> Handle(UserExistsQuery query, CancellationToken cancellationToken = default)
    {
        var exists = await _usersService.UserExistsAsync(query.UserId, cancellationToken);

        return exists;
    }
}
