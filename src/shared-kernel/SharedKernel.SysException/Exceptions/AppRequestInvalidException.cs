namespace SharedKernel.SysException.Exceptions;

public class AppRequestInvalidException(
    ErrorCode errorCode,
    string title,
    string? errorField = null
) : AppInvalidException
{
    public override ErrorCode ErrorCode => errorCode;
    public override string Title => title;
    public override string? ErrorField => errorField;
}
