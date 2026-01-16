using PlatformKernel.ApplicationShared.Extensions;

namespace PlatformKernel.SysException.Exceptions;

public class TooManyConcurrentUploadException : AppInvalidException
{
    public override ErrorCode ErrorCode => ErrorCode.E0104;
    public override string Title => ErrorCode.GetEnumDescriptions();
}
