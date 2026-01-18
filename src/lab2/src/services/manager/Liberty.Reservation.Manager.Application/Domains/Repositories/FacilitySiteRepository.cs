namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilitySiteRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilitySite>(dataContext), IFacilitySiteRepository;
