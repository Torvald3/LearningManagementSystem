namespace LMS.Identity.Api.Models;

public sealed record ConfirmEmailRequest(
    Guid UserId, 
    string Token);