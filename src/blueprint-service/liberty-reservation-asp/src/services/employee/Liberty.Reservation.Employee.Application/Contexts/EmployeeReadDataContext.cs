namespace Liberty.Reservation.Employee.Application.Contexts;

public class EmployeeReadDataContext(
    DbContextOptions<EmployeeReadDataContext> options
) : EmployeeDataContext(options);
