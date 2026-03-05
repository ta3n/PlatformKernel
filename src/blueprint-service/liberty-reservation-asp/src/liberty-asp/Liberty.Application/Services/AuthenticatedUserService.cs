using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Liberty.Application.Services;

public interface IAuthenticatedUserService
{
    string UserCode { get; }
}
public class AuthenticatedUserService : IAuthenticatedUserService
{
    IHttpContextAccessor _httpContextAccessor;
    public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
    {
        UserCode = httpContextAccessor.HttpContext?.User?.FindFirstValue("code");
    }

    public string UserCode { get;}
}
