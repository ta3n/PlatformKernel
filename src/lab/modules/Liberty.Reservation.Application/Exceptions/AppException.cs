using Liberty.Reservation.Application.Constants;
using System.Data.Common;

namespace Liberty.Reservation.Application.Exceptions;

[Serializable]
public abstract class AppException : Exception
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
        Exception? innerException
    ) : base(message, innerException)
    {
    }

    public static AppException GetAppException(
        Exception e
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
