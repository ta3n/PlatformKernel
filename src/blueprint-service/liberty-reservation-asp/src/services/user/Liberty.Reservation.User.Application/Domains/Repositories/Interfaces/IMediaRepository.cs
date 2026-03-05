using Liberty.UnitOfWork.Abstractions;
using File = Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data.File;

namespace Liberty.Reservation.User.Application.Domains.Repositories.Interfaces;

public interface IMediaRepository : IRepositoryBase<File>;
