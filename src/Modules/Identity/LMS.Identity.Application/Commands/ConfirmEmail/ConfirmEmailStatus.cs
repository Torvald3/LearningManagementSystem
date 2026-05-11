namespace LMS.Identity.Application.Commands.ConfirmEmail;

public enum ConfirmEmailStatus
{
    Success,
    UserNotFound,
    AlreadyConfirmed,
    InvalidToken
}