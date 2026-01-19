using System.Security.Authentication;

namespace BlueprintCqrs.Crosscutting.Exceptions;

public class UserNotActivatedException(
    string message
) : AuthenticationException(message)
{
}
