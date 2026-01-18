namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityAppDateTypeRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilityAppDateType>(dataContext), IFacilityAppDateTypeRepository;
