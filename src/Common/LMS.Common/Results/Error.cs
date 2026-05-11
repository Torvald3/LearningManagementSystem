namespace LMS.Common.Results;

public readonly record struct Error(string Code, string Message, ErrorType Type)
{
    public static Error None => new(string.Empty, string.Empty, ErrorType.None);

    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
}
