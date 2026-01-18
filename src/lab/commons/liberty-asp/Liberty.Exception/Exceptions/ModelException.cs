namespace Liberty.Exception.Exceptions;

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
        System.Exception ex
    ) : base(message, ex)
    {
    }
}
