using Liberty.Reservation.Application.Constants;

namespace Liberty.Reservation.Application.Exceptions;

public abstract class ModelException : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.X00094;

    public override string Title => "モデルエラー";

    public override string ApproachMessage => "";

    protected ModelException() : base("モデル情報にエラーがあります")
    {
    }

    protected ModelException(
        string message
    ) : base(message)
    {
    }

    protected ModelException(
        string message,
        Exception ex
    ) : base(message, ex)
    {
    }
}
