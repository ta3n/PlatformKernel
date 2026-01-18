namespace Liberty.Reservation.Employee.Application.Constants;

public enum EmployeeFileTypes
{
    Default = 1 << 0,
    Avatar = 1 << 1
}

public enum EmployeeStatus
{
    Created = 1 << 0,
    Cancelled = 1 << 1,
    EmailConfirmed = 1 << 2
}
