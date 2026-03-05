namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityOptionItemRepository(
    ManagerDataContext dbContext
) : RepositoryBase<FacilityOptionItem>(dbContext), IFacilityOptionItemRepository;
