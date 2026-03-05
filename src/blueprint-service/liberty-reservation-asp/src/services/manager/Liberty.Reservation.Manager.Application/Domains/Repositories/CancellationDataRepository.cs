namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class CancellationDataRepository(
    ManagerDataContext dataContext
) : RepositoryBase<CancellationData>(dataContext), ICancellationDataRepository;
