namespace LMS.Users.Core.Services;

public interface IEmailService
{
    Task SendUserConfirmationEmailAsync(string email, string token);
}