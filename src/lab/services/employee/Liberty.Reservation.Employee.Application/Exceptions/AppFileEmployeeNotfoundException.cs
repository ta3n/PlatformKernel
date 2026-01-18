using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Employee.Application.Exceptions;

[Serializable]
public class AppFileEmployeeNotfoundException(
    string fileCode,
    string employeeCode
) : AppNotfoundException
{
    public override ErrorCode ErrorCode => ErrorCode.E0000x;

    public override string Title => $"File of employee not found at {FileCode} {EmployeeCode}";

    public string? FileCode { get; } = fileCode;
    public string? EmployeeCode { get; } = employeeCode;
}
