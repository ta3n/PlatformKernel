using SharedKernel.AppShared.Extensions;

namespace SharedKernel.Exception.Exceptions;

public class AppUnknownErrorException(
    System.Exception exception
) : AppUnknownException
{
    public override ErrorCode ErrorCode => ErrorCode.E0100;

    public override string Title => ErrorCode.GetEnumDescriptions();

    public System.Exception Exception { get; } = exception;
}
