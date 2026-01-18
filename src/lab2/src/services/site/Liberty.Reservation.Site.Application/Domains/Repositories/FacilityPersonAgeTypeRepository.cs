namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class FacilityPersonAgeTypeRepository(
    SiteDataContext dataContext
) : RepositoryBase<FacilityPersonAgeType>(dataContext), IFacilityPersonAgeTypeRepository;
