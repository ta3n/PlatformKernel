namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityRoomGroupRepository(
    ManagerDataContext dbContext
) : RepositoryBase<FacilityRoomGroup>(dbContext), IFacilityRoomGroupRepository;
