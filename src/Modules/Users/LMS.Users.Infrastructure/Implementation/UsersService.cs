using LMS.Users.Core.Models;
using LMS.Users.Core.Services;
using LMS.Users.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace LMS.Users.Infrastructure.Implementation;

public class UsersService : IUsersService
{
    private readonly UsersDbContext _context;

    public UsersService(UsersDbContext context)
    {
        _context = context;
    }


    public async Task CreateUserAsync(User user)
    {
        _context.Users.Add(new Entities.User()
        {
            Id = user.Id,
            AvatarMediaId = user.AvatarMediaId,
            Bio = user.Bio,
            Username = user.Username,
            Contacts = new Entities.Contacts()
            {
                Email = user.Contacts.Email
            }
        });
        
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .AsNoTracking()
            .Include(x => x.Contacts)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new User
        {
            Id = user.Id,
            AvatarMediaId = user.AvatarMediaId,
            Bio = user.Bio,
            Username = user.Username,
            Contacts = new Contacts
            {
                Email = user.Contacts.Email,
                Phone = user.Contacts.Phone
            }
        };
    }

    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken);

        if (existingUser is null)
        {
            return false;
        }

        existingUser.Bio = user.Bio;
        existingUser.AvatarMediaId = user.AvatarMediaId;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(x => x.Id == userId, cancellationToken);
    }
}

