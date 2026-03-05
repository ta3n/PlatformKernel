using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class SystemConfigRepository(
    EmployeeDataContext dbContext
) : RepositoryBase<SystemConfig>(dbContext), ISystemConfigRepository;
