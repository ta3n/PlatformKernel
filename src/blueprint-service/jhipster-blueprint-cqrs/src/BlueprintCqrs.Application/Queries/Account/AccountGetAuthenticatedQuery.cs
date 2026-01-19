using System.Security.Claims;
using MediatR;

namespace BlueprintCqrs.Application.Queries.Account;

public class AccountGetAuthenticatedQuery : IRequest<string>
{
    public ClaimsPrincipal User { get; set; }
}
