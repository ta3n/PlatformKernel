using Liberty.Cache.Services;
using Liberty.Reservation.Application.Domains.Services;
using Liberty.Reservation.Employee.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Employee.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Employee.Application.Exceptions;
using Microsoft.Extensions.Logging;

namespace Liberty.Reservation.Employee.Application.Domains.Services;

public class ConsumptionTaxService(
    ILogger<ConsumptionTaxService> logger,
    ICacheService cacheService,
    IConsumptionTaxRepository consumptionTaxRepository
) : BaseService<ConsumptionTax>(logger, cacheService, consumptionTaxRepository, new ConsumptionTaxNotfoundException()),
    IConsumptionTaxService;
