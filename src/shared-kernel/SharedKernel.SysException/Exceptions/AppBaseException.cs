namespace SharedKernel.SysException.Exceptions;

public class AppBaseException : AppException
{
    public override ErrorCode ErrorCode { get; }
    public override string Title { get; }

    public AppBaseException(
        string message
    ) : base(message)
    {
        ErrorCode = ErrorCode.E0100;
        Title = message;
    }

    public AppBaseException(
        ErrorCode errorCode,
        string message
    ) : base(message)
    {
        ErrorCode = errorCode;
        Title = message;
    }

    public AppBaseException(
        string message,
        Exception innerException
    ) : base(message, innerException)
    {
        ErrorCode = ErrorCode.E0100;
        Title = message;
    }

    public AppBaseException(
        ErrorCode errorCode,
        string message,
        Exception innerException
    ) : base(message, innerException)
    {
        ErrorCode = errorCode;
        Title = message;
    }
}
