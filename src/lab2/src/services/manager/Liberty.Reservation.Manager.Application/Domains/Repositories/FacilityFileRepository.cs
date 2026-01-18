namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FacilityFileRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FacilityFile>(dataContext), IFacilityFileRepository;
