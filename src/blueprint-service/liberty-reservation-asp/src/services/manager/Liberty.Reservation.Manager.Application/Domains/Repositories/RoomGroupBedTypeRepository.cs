namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class RoomGroupBedTypeRepository(
    ManagerDataContext dbContext
) : RepositoryBase<RoomGroupBedType>(dbContext), IRoomGroupBedTypeRepository;
