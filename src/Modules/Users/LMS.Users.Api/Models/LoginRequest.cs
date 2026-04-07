namespace LMS.Users.Api.Models;

public record LoginRequest(string EmailOrUsername, string Password);