using LMS.Common.CQRS;
using LMS.Users.Application.Commands;
using LMS.Users.Contracts.Models;
using LMS.Users.Contracts.Services;

namespace LMS.Users.Application.Services;

internal class UsersModuleService : IUsersModuleService
{
    private readonly ICommandHandler<CreateUserCommand> _createUserCommandHandler;

    public UsersModuleService(ICommandHandler<CreateUserCommand> createUserCommandHandler)
    {
        _createUserCommandHandler = createUserCommandHandler;
    }

    public Task CreateUserAsync(CreateUserRequest request)
    {
        return _createUserCommandHandler.HandleAsync(
            new CreateUserCommand(request.UserId, request.Email, request.Username));
    }
}