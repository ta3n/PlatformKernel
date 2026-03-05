namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityAllergenRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilityAllergen>(dataContext), IFacilityAllergenRepository;
