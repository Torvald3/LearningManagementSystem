using LMS.Common.CQRS;
using LMS.Common.Results;
using LMS.Users.Application.Errors;
using LMS.Users.Application.Models;
using LMS.Users.Core.Services;

namespace LMS.Users.Application.Queries;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, User>
{
    private readonly IUsersService _usersService;

    public GetUserQueryHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task<Result<User>> Handle(GetUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await _usersService.GetUserAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.UserNotFound(query.UserId);
        }

        return new User(
            user.Id,
            user.Username,
            user.Contacts.Email,
            user.Bio,
            user.AvatarMediaId);
    }
}
