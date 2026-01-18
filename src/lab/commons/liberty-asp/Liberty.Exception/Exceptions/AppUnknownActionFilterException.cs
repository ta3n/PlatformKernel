namespace Liberty.Exception.Exceptions;

public class AppUnknownActionFilterException(
    System.Exception exception
) : AppUnknownException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "Unknown Error";

    public System.Exception Exception { get; } = exception;
}
