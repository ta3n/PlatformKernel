namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class DataOfCancellationRepository(
    ManagerDataContext dataContext
) : RepositoryBase<CancellationCancellationData>(dataContext), IDataOfCancellationRepository;
