using Liberty.ApplicationShared.Extensions;

namespace Liberty.SysException.Exceptions;

public class AppUnknownErrorException(
    Exception exception
) : AppUnknownException
{
    public override ErrorCode ErrorCode => ErrorCode.E0100;

    public override string Title => ErrorCode.GetEnumDescriptions();

    public Exception Exception { get; } = exception;
}
