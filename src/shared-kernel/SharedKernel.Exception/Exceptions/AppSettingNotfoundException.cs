namespace SharedKernel.Exception.Exceptions;

public class AppSettingNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E0102;

    public override string Title => "No application setting";
}
