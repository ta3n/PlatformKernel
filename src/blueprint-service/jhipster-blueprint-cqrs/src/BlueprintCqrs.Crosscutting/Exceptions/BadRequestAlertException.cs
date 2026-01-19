using BlueprintCqrs.Crosscutting.Constants;

namespace BlueprintCqrs.Crosscutting.Exceptions;

public class BadRequestAlertException(
    string type,
    string detail,
    string entityName,
    string errorKey
) : BaseException(type, detail, entityName, errorKey)
{
    public BadRequestAlertException(
        string detail,
        string entityName,
        string errorKey
    ) : this(
        ErrorConstants.DefaultType,
        detail,
        entityName,
        errorKey
    )
    {
    }
}
