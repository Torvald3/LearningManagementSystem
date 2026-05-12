namespace LMS.Common.Authorization;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }
}
