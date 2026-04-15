using LMS.Identity.Core.Models;

namespace LMS.Identity.Core.TokenGenerator;

public interface IAccessTokenGenerator
{
    AccessTokenResult Generate(ApplicationUser user);
}