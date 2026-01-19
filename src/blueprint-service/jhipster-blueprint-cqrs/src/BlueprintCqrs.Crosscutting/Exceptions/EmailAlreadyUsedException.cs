using BlueprintCqrs.Crosscutting.Constants;

namespace BlueprintCqrs.Crosscutting.Exceptions;

public class EmailAlreadyUsedException : BadRequestAlertException
{
    public EmailAlreadyUsedException() : base(
        ErrorConstants.EmailAlreadyUsedType,
        "Email is already in use!",
        "userManagement",
        "emailexists"
    )
    {
    }
}
