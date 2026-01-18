using Liberty.ApplicationShared.Attributes;
using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Application.Extensions;

public static class EnumExtension
{
    public static Exception GetException(
        this ErrorCode errorCode
    )
    {
        var memberInfos = errorCode.GetType().GetMember(errorCode.ToString());
        var attributes = memberInfos[0].GetCustomAttributes(typeof(ExceptionAttribute), false);
        var errorType = ((ExceptionAttribute)attributes[0]).ErrorType;

        if (typeof(Exception).IsAssignableFrom(errorType))
        {
            // インスタンスを生成して throw する
            return (Exception)Activator.CreateInstance(errorType)!;
        }

        throw new ArgumentException($"Invalid errorCode:{errorCode}");
    }
}
