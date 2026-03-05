namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class BedTypeRepository(
    ManagerDataContext dbContext
) : RepositoryBase<BedType>(dbContext), IBedTypeRepository;
