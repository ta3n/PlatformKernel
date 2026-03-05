namespace Liberty.Reservation.Manager.Application.Domains.Repositories;

public class SystemConfigRepository(
    ManagerDataContext dbContext
) : RepositoryBase<SystemConfig>(dbContext), ISystemConfigRepository;
