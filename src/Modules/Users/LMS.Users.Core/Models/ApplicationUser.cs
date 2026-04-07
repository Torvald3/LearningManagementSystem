using Microsoft.AspNetCore.Identity;

namespace LMS.Users.Core.Models;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? AvatarUrl { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? UpdatedAt { get; set; }
}