using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class FaxSrvService(
    ILogger<FaxSrvService> logger,
    ICacheService cacheService,
    IFaxServiceRepository faxServiceRepository
) : BaseService<FaxService>(logger, cacheService, faxServiceRepository, new FaxServiceNotfoundException()),
    IFaxSrvService;
