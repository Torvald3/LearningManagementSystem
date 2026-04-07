namespace LMS.Users.Api.Models;

public sealed record ConfirmEmailRequest(
    Guid UserId, 
    string Token);