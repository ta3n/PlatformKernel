using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class ConsumptionTaxRepository(
    EmployeeDataContext dbContext
) : RepositoryBase<ConsumptionTax>(dbContext), IConsumptionTaxRepository;
