using System;

namespace BlueprintCqrs.Crosscutting.Exceptions;

public abstract class BaseException(
    string type,
    string detail,
    string entityName,
    string errorKey
) : Exception(detail)
{
    public BaseException(
        string type,
        string detail
    ) : this(type, detail, null, null)
    {
    }

    public BaseException(
        string type,
        string detail,
        string entityName
    ) : this(type, detail, entityName, null)
    {
    }

    public string Type { get; set; } = type;
    public string Detail { get; set; } = detail;
    public string EntityName { get; set; } = entityName;
    public string ErrorKey { get; set; } = errorKey;
}
