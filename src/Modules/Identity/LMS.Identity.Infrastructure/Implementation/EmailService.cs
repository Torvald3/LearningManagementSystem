using LMS.Identity.Core.Services;
using Microsoft.Extensions.Logging;

namespace LMS.Identity.Infrastructure.Implementation;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendUserConfirmationEmailAsync(string email, string token)
    {
        _logger.LogInformation("Email confirmation requested for {Email}. Sending is not implemented yet.", email);
        
        return Task.CompletedTask;
    }
}