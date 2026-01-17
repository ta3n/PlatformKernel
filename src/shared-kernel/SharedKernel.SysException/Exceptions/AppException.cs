using System.Data.Common;

namespace SharedKernel.SysException.Exceptions;

public abstract class AppException : Exception
{
    public abstract ErrorCode ErrorCode { get; }

    public abstract string Title { get; }

    public virtual string? ErrorField => null;

    public virtual string ErrorCaption => string.Empty;
    public virtual string ApproachMessage => string.Empty;
    public virtual bool IsNotFound => false;

    protected AppException()
    {
    }

    protected AppException(
        string? message
    ) : base(message)
    {
    }

    protected AppException(
        string? message,
        Exception? innerException
    ) : base(message, innerException)
    {
    }

    public static AppException GetAppException(
        Exception ex
    )
    {
        if (ex is DbException dbException)
        {
            return new AppDbException(dbException);
        }

        if (ex is AppBaseException appBaseException)
        {
            return appBaseException;
        }

        if (ex is AppNotfoundException appNotfoundException)
        {
            return appNotfoundException;
        }

        if (ex is not AppException appException)
        {
            return new AppUnknownErrorException(ex);
        }

        return appException;
    }
}
