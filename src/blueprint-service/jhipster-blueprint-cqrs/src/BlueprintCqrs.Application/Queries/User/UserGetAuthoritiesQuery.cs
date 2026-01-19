using System.Collections.Generic;
using MediatR;

namespace BlueprintCqrs.Application.Queries.User;

public class UserGetAuthoritiesQuery : IRequest<IEnumerable<string>>
{
}
