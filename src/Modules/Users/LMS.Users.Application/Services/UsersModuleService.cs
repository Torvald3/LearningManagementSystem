using LMS.Common.CQRS;
using LMS.Users.Application.Commands;
using LMS.Users.Application.Queries;
using LMS.Users.Contracts.Models;
using LMS.Users.Contracts.Services;

namespace LMS.Users.Application.Services;

internal class UsersModuleService : IUsersModuleService
{
    private readonly ICommandHandler<CreateUserCommand> _createUserCommandHandler;
    private readonly IQueryHandler<UserExistsQuery, bool> _userExistsQueryHandler;

    public UsersModuleService(
        ICommandHandler<CreateUserCommand> createUserCommandHandler,
        IQueryHandler<UserExistsQuery, bool> userExistsQueryHandler)
    {
        _createUserCommandHandler = createUserCommandHandler;
        _userExistsQueryHandler = userExistsQueryHandler;
    }

    public async Task CreateUserAsync(CreateUserRequest request)
    {
        var result = await _createUserCommandHandler.HandleAsync(
            new CreateUserCommand(request.UserId, request.Email, request.Username));

        if (result.IsFailure)
        {
            throw new InvalidOperationException(result.Error.Message);
        }
    }

    public async Task<bool> UserExistsAsync(Guid userId)
    {
        var result = await _userExistsQueryHandler.Handle(new UserExistsQuery(userId));

        return result.Value;
    }
}
