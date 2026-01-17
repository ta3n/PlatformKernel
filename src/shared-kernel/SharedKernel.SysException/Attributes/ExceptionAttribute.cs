namespace SharedKernel.SysException.Attributes;

[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Enum
    | AttributeTargets.Interface
    | AttributeTargets.Delegate
)]
public class ExceptionAttribute(
    Type type
) : Attribute
{
    public Type ErrorType { get; } = type;
}
