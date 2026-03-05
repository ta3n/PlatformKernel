namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class AllergenRepository(
    ManagerDataContext dataContext
) : RepositoryBase<Allergen>(dataContext), IAllergenRepository;
