using Liberty.Reservation.Application.Exceptions;

namespace Liberty.Reservation.Manager.Application.Domains.Services;

public class PriceDataService(
    ILogger<PriceData> logger,
    IPriceDataRepository priceDataRepository
)
    : BaseService<PriceData>(logger, priceDataRepository, new PriceDataNotfoundException()),
        IPriceDataService;
