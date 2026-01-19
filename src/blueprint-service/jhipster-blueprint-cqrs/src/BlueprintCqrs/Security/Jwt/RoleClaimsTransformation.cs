using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace BlueprintCqrs.Security.Jwt;

public class RoleClaimsTransformation(
    ITokenProvider tokenProvider
) : IClaimsTransformation
{
    private readonly ITokenProvider _tokenProvider = tokenProvider;

    public Task<ClaimsPrincipal> TransformAsync(
        ClaimsPrincipal principal
    )
    {
        return Task.FromResult(_tokenProvider.TransformPrincipal(principal));
    }
}
