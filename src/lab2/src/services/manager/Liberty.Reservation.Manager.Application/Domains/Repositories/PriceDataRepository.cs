namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PriceDataRepository(
    ManagerDataContext dbContext
) : RepositoryBase<PriceData>(dbContext), IPriceDataRepository;
