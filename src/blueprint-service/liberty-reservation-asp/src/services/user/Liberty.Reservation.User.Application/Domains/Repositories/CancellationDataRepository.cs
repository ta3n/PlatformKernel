using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.User.Application.Domains.Repositories;

public class CancellationDataRepository(
    UserDataContext dataContext
) : RepositoryBase<CancellationData>(dataContext), ICancellationDataRepository;
