namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class PlanRoomGroupRepository(
    ManagerDataContext dataContext
) : RepositoryBase<PlanRoomGroup>(dataContext), IPlanRoomGroupRepository;
