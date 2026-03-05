using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.UnitOfWork.Abstractions;

namespace Liberty.Reservation.Application.Domains.Repositories.Interfaces;

public interface IBookingRoomRepresentativeRepository : IRepositoryBase<ReservationPlanRoomGroupAppDate>;
