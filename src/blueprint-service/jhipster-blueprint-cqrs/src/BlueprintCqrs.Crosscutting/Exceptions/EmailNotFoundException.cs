using BlueprintCqrs.Crosscutting.Constants;

namespace BlueprintCqrs.Crosscutting.Exceptions;

public class EmailNotFoundException : BaseException
{
    public EmailNotFoundException() : base(ErrorConstants.EmailNotFoundType, "Email address not registered")
    {
    }
}
