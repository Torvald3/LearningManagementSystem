using LMS.Users.Core.Models;

namespace LMS.Users.Core.Services;

public interface IUsersService
{
    Task CreateUserAsync(User user);

    Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
