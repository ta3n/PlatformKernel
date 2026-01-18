namespace Liberty.SysException.Exceptions;

public abstract class ModelException : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E0100;

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
