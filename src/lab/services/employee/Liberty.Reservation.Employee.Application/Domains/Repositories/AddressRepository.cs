using Liberty.Reservation.Employee.Application.Contexts;
using Liberty.Reservation.Employee.Application.Contexts.Entities;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Repositories;

public class AddressRepository(
    ILogger<AddressRepository> logger,
    EmployeeDataContext dbContext
) : RepositoryBase<Address>(logger, dbContext), IAddressRepository;
