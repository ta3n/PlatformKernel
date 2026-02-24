using SharedKernel.AppShared.Extensions;

namespace SharedKernel.Exception.Exceptions;

public class TooManyConcurrentUploadException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E0104;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
