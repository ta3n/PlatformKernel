using SharedKernel.ApplicationShared.Extensions;

namespace SharedKernel.SysException.Exceptions;

public class TooManyConcurrentUploadException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E0104;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
