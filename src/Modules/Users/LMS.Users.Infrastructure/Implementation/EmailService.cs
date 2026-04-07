using LMS.Users.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Users.Infrastructure.Implementation;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> logger;

    public EmailService(ILogger<EmailService> logger)
    {
        this.logger = logger;
    }

    public Task SendUserConfirmationEmailAsync(string email, string token)
    {
        logger.LogInformation("Email confirmation requested for {Email}. Sending is not implemented yet.", email);
        
        return Task.CompletedTask;
    }
}