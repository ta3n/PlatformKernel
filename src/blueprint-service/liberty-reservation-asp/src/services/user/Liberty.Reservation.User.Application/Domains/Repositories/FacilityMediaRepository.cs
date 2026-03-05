using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;

namespace Liberty.Reservation.User.Application.Domains.Repositories;

public class FacilityMediaRepository(
    UserDataContext dbContext
) : RepositoryBase<FacilityFile>(dbContext), IFacilityMediaRepository;
