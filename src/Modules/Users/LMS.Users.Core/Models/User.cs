namespace LMS.Users.Core.Models;

public class User
{
    public Guid Id { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }
    
    public Contacts Contacts { get; set; } = new();
}