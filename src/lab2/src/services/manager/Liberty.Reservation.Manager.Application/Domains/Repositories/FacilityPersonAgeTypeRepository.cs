namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityPersonAgeTypeRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilityPersonAgeType>(dataContext), IFacilityPersonAgeTypeRepository;
