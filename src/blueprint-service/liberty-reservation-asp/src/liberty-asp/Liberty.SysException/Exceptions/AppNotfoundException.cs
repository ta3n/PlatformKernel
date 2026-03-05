namespace Liberty.SysException.Exceptions;

public abstract class AppNotfoundException : AppException
{
    public override ErrorCode ErrorCode => ErrorCode.E0101;
}
