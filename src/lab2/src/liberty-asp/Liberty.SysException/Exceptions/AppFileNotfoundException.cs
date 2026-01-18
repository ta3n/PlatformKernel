using Liberty.ApplicationShared.Extensions;

namespace Liberty.SysException.Exceptions;

public class AppFileNotfoundException(
    string fileCode
) : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E1004;

    public override string Title => string.Format(
        ErrorCode.GetEnumDescriptions(),
        fileCode
    );
}
