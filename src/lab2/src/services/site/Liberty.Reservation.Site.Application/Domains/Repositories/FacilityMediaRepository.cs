namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class FacilityMediaRepository(
    SiteDataContext dbContext
) : RepositoryBase<FacilityFile>(dbContext), IFacilityMediaRepository;
