using SharedKernel.SysException.Attributes;

namespace SharedKernel.SysException.Extensions;

public static class EnumExtension
{
    public static Exception GetException(
        this ErrorCode errorCode
    )
    {
        var memberInfos = errorCode.GetType().GetMember(errorCode.ToString());
        var attributes = memberInfos[0].GetCustomAttributes(typeof(ExceptionAttribute), false);
        var errorType = ((ExceptionAttribute)attributes[0]).ErrorType;

        if (typeof(Exceptions.AppException).IsAssignableFrom(errorType))
        {
            // インスタンスを生成して throw する
            return (Exceptions.AppException)Activator.CreateInstance(errorType)!;
        }

        throw new ArgumentException($"Invalid errorCode:{errorCode}");
    }
}
