namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityQuestionRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilityQuestion>(dataContext), IFacilityQuestionRepository;
