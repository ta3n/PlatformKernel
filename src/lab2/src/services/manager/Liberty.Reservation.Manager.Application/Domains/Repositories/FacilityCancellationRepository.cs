namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityCancellationRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilityCancellation>(dataContext), IFacilityCancellationRepository;
