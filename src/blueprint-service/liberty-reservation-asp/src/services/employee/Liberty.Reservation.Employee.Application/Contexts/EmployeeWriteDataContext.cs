namespace Liberty.Reservation.Employee.Application.Contexts;

public class EmployeeWriteDataContext(
    DbContextOptions<EmployeeWriteDataContext> options
) : EmployeeDataContext(options);
