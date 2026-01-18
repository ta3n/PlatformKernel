namespace Liberty.Exception.Exceptions;

public class AppUnknownErrorException : AppUnknownException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "Unknown Error";
}
