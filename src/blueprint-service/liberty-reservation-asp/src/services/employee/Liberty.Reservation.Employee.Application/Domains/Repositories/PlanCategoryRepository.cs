using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class PlanCategoryRepository(
    EmployeeDataContext dataContext
) : RepositoryBase<PlanCategory>(dataContext), IPlanCategoryRepository;
