using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingRoomGroupRepository(
    DbContext dataContext
) : RepositoryBase<RoomGroup>(dataContext), IBookingRoomGroupRepository;
