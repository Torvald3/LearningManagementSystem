namespace LMS.Users.Infrastructure.Entities;

public class Contacts
{
    public Guid Id { get; set; }
    
    public required string Email { get; set; } 
    
    public string? Phone { get; set; } 
}