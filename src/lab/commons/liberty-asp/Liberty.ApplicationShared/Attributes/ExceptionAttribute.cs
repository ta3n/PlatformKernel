namespace Liberty.ApplicationShared.Attributes;

public class ExceptionAttribute(
    Type type
) : Attribute
{
    public Type ErrorType { get; } = type;
}
