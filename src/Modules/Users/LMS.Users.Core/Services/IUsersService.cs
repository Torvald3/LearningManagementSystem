using LMS.Users.Core.Models;

namespace LMS.Users.Core.Services;

public interface IUsersService
{
    Task CreateUserAsync(User user);
}