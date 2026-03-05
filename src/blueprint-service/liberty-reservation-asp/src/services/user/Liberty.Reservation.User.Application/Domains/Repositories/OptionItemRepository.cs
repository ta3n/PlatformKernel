using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.User.Application.Domains.Repositories;

public class OptionItemRepository(
    UserDataContext dbContext
) : RepositoryBase<OptionItem>(dbContext), IOptionItemRepository;
