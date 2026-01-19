using BlueprintNormal.Crosscutting.Constants;

namespace BlueprintNormal.Crosscutting.Exceptions;

public class EmailNotFoundException : BaseException
{
    public EmailNotFoundException() : base(ErrorConstants.EmailNotFoundType, "Email address not registered")
    {
    }
}
