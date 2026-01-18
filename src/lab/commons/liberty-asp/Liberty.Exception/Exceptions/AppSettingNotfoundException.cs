namespace Liberty.Exception.Exceptions;

public class AppSettingNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "No application setting";
}
