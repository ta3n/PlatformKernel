using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.User.Application.Domains.Repositories;

public class MediaRepository(
    UserDataContext dbContext
) : RepositoryBase<File>(dbContext), IMediaRepository;
