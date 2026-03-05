using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingRoomRepresentativeRepository(
    DbContext dataContext
) : RepositoryBase<ReservationPlanRoomGroupAppDate>(dataContext), IBookingRoomRepresentativeRepository;
