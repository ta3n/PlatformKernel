namespace Liberty.Reservation.Site.Application.Domains.Repositories;

public class FacilityRoomGroupRepository(
    SiteDataContext dbContext
) : RepositoryBase<FacilityRoomGroup>(dbContext), IFacilityRoomGroupRepository;
