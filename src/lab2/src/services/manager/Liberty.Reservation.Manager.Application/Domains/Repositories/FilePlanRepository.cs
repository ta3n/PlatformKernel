namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class FilePlanRepository(
    ManagerDataContext dataContext
) : RepositoryBase<FilePlan>(dataContext), IFilePlanRepository;
