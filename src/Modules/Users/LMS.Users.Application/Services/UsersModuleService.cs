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

    public UsersModuleService(ICommandHandler<CreateUserCommand> createUserCommandHandler, 
                              IQueryHandler<UserExistsQuery, bool> userExistsQueryHandler)
    {
        _createUserCommandHandler = createUserCommandHandler;
        _userExistsQueryHandler = userExistsQueryHandler;
    }

    public Task CreateUserAsync(CreateUserRequest request)
    {
        return _createUserCommandHandler.HandleAsync(
            new CreateUserCommand(request.UserId, request.Email, request.Username));
    }

    public Task<bool> UserExistsAsync(Guid userId)
    {
        return _userExistsQueryHandler.Handle(new UserExistsQuery(userId));
    }
}