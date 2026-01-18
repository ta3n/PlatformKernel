namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class CancellationRepository(
    ManagerDataContext dataContext
) : RepositoryBase<Cancellation>(dataContext), ICancellationRepository;
