using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Relations;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.UnitOfWork.Implementations;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Repositories;

public class BookingPlanRoomGroupSiteDiscountDataRepository(
    DbContext dbContext
) : RepositoryBase<PlanRoomGroupSiteDiscountData>(dbContext), IBookingPlanRoomGroupSiteDiscountDataRepository;
