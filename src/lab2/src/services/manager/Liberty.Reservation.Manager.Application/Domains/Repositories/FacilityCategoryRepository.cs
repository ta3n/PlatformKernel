namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityCategoryRepository(
    ManagerDataContext dbContext
) : RepositoryBase<FacilityCategory>(dbContext), IFacilityCategoryRepository;
