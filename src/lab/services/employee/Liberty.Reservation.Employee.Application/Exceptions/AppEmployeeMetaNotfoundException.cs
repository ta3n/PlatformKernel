using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

[Serializable]
public class AppEmployeeMetaNotfoundException : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => $"Employee not found at {Code}";

    public string? Code { get; }

    public AppEmployeeMetaNotfoundException()
    {
    }

    public AppEmployeeMetaNotfoundException(
        string? code
    )
    {
        Code = code;
    }
}
