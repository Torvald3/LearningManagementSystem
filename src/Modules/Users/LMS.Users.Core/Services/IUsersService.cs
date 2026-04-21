using LMS.Users.Core.Models;

namespace LMS.Users.Core.Services;

public interface IUsersService
{
    Task CreateUserAsync(User user);
    
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}