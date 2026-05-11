namespace LMS.Identity.Application.Commands.ConfirmEmail;

public record ConfirmEmailResult(
    ConfirmEmailStatus Status,
    IEnumerable<string> Errors);