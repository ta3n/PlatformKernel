using System.Security.Authentication;

namespace BlueprintCqrs.Crosscutting.Exceptions;

public class UsernameNotFoundException(
    string message
) : AuthenticationException(message)
{
}
