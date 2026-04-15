using LMS.Common.CQRS;
using LMS.Users.Core.Models;
using LMS.Users.Core.Services;

namespace LMS.Users.Application.Commands;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUsersService _usersService;

    public CreateUserCommandHandler(IUsersService usersService)
    {
        _usersService = usersService;
    }

    public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        await _usersService.CreateUserAsync(new User()
        {
            Id = command.UserId,
            AvatarUrl = null,
            Username = command.Username,
            Contacts = new Contacts()
            {
                Email = command.Email
            },
            Bio = null
        });
    }
}