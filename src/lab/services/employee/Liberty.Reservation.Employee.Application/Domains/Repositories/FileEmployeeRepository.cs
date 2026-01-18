using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Contexts.Entities.Relations;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class FileEmployeeRepository(
    ILogger<FileEmployeeRepository> logger,
    EmployeeDataContext dbContext
) : RepositoryBase<FileEmployee>(logger, dbContext), IFileEmployeeRepository;
