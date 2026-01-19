using BlueprintCqrs.Crosscutting.Constants;

namespace BlueprintCqrs.Crosscutting.Exceptions;

public class InternalServerErrorException(
    string message
) : BaseException(ErrorConstants.DefaultType, message)
{
}
