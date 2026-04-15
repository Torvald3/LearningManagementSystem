namespace LMS.Users.Infrastructure.Entities;

public class User
{
    public Guid Id { get; set; }
    
    public Guid ContactsId { get; set; }
    
    public string Username { get; set; }
    
    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }
    
    public Contacts Contacts { get; set; }
}