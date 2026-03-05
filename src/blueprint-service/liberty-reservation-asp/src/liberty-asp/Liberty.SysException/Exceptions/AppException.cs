using System.Data.Common;

namespace Liberty.SysException.Exceptions;

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
        // SQLExceptionの場合ここで判別しApp例外に変換
        if (ex is DbException dbException)
        {
            return new AppDbException(dbException);
        }

        if (ex is AppLibertyException appLibertyException)
        {
            return appLibertyException;
        }

        if (ex is AppNotfoundException appNotfoundException)
        {
            return appNotfoundException;
        }

        // 判別できない例外
        if (ex is not AppException appException)
        {
            return new AppUnknownErrorException(ex);
        }

        // ここまでで発生した例外をAppExceptionに回収できているためキャスト

        return appException;
    }
}
