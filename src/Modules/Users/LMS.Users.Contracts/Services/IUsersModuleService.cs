using LMS.Users.Contracts.Models;

namespace LMS.Users.Contracts.Services;

public interface IUsersModuleService
{
    Task CreateUserAsync(CreateUserRequest request);
}