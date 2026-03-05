using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.User.Application.Domains.Repositories;

public class SystemConfigRepository(
    UserDataContext dbContext
) : RepositoryBase<SystemConfig>(dbContext), ISystemConfigRepository;
