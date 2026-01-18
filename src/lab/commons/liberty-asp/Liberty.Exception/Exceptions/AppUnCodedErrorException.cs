namespace Liberty.Exception.Exceptions;

public class AppUnCodedErrorException : AppUnknownException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => "UnCoded Error";
}
