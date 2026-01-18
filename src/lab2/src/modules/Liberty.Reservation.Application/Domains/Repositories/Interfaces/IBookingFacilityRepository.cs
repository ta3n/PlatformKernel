using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

public interface IBookingFacilityRepository : IRepositoryBase<Facility>;
