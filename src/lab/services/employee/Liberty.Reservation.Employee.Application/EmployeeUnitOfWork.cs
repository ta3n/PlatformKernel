using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Employee.Application;

public class EmployeeUnitOfWork(
    DbContext context
) : BaseUnitOfWork(context), IEmployeeUnitOfWork;
