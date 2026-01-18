using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class EmployeeMetaRepository(
    ILogger<EmployeeMetaRepository> logger,
    EmployeeDataContext dbContext
) : RepositoryBase<EmployeeMeta>(logger, dbContext), IEmployeeMetaRepository;
