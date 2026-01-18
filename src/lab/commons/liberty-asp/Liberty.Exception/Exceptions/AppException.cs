using System.Data.Common;

namespace Liberty.Exception.Exceptions;

[Serializable]
public abstract class AppException : System.Exception
{
    public abstract ErrorCode ErrorCode { get; }

    public abstract string Title { get; }

    public virtual string ErrorCaption => "";
    public virtual string ApproachMessage => "";
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
        System.Exception? innerException
    ) : base(message, innerException)
    {
    }

    public static AppException GetAppException(
        System.Exception e
    )
    {
        // SQLExceptionの場合ここで判別しApp例外に変換
        if (e is DbException exception)
        {
            return new AppDbException(exception);
        }

        // 判別できない例外
        if (e is not AppException appException)
        {
            return new AppUnknownActionFilterException(e);
        }

        // ここまでで発生した例外をAppExceptionに回収できているためキャスト

        return appException;
    }
}
