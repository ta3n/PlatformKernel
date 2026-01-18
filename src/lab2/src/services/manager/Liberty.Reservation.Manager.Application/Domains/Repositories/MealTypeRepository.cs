namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class MealTypeRepository(
    ManagerDataContext dbContext
) : RepositoryBase<MealType>(dbContext), IMealTypeRepository;
