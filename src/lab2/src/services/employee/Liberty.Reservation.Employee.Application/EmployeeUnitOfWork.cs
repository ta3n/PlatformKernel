using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.Employee.Application;

public class EmployeeUnitOfWork(
    DbContext context
) : BaseUnitOfWork(context), IEmployeeUnitOfWork;
