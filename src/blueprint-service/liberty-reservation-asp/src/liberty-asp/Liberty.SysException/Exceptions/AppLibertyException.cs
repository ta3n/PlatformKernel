namespace Liberty.SysException.Exceptions;

public class AppLibertyException : AppException
{
    public override ErrorCode ErrorCode { get; }
    public override string Title { get; }

    public AppLibertyException(
        string message
    ) : base(message)
    {
        ErrorCode = ErrorCode.E0100;
        Title = message;
    }

    public AppLibertyException(
        ErrorCode errorCode,
        string message
    ) : base(message)
    {
        ErrorCode = errorCode;
        Title = message;
    }

    public AppLibertyException(
        string message,
        Exception innerException
    ) : base(message, innerException)
    {
        ErrorCode = ErrorCode.E0100;
        Title = message;
    }

    public AppLibertyException(
        ErrorCode errorCode,
        string message,
        Exception innerException
    ) : base(message, innerException)
    {
        ErrorCode = errorCode;
        Title = message;
    }
}
