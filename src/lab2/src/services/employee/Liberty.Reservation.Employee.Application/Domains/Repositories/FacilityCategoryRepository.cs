using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class FacilityCategoryRepository(
    EmployeeDataContext dbContext
) : RepositoryBase<FacilityCategory>(dbContext), IFacilityCategoryRepository;
