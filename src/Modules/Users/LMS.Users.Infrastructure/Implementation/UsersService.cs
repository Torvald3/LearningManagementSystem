using LMS.Users.Core.Models;
using LMS.Users.Core.Services;
using LMS.Users.Infrastructure.DbContexts;

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
            AvatarUrl = user.AvatarUrl,
            Bio = user.Bio,
            Username = user.Username,
            Contacts = new Entities.Contacts()
            {
                Email = user.Contacts.Email
            }
        });
        
        await _context.SaveChangesAsync();
    }
}