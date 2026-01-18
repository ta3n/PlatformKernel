using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Domains.Repositories.Interfaces;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Liberty.Reservation.Application.Domains.Services;

public class BookingInventoryService(
    IBookingDataPlanRepository dataPlanRepo,
    IBookingRoomGroupAppDateRepository roomGroupAppDateRepo
) : IBookingInventoryService
{
    public async Task<IEnumerable<RoomInventoryModel>> GetRoomInventoryAsync(
        BookingHoldCheckModel model,
        CancellationToken cancellationToken = default
    )
    {
        var planQueryable = dataPlanRepo
            .GetQueryableWithAsNoTracking()
            .Where(x => x.Id == model.PlanId)
            .Where(x => x.IsEnabled)
            .Where(x => x.IsOnLinePayment || x.IsOnSidePayment)
            .Where(x => x.Cancellation!.IsEnabled)
            .Where(
                x =>
                    x.UseDaySaleLimit
                    && x.PlanDaySaleLimitType == PlanDaySaleLimitTypes.RoomGroup
            )
            .Select(
                x => new
                {
                    x.Id,
                    x.RoomNumberDaySaleLimit
                }
            )
            .AsSingleQuery();

        var existingPlan = await planQueryable.FirstOrDefaultAsync(cancellationToken);
        var limit = existingPlan?.RoomNumberDaySaleLimit;
        if (limit < model.NumberOfRooms)
        {
            limit = 0;
        }

        var roomInventoryQueryable = roomGroupAppDateRepo
            .GetQueryableWithAsNoTracking()
            .Where(x => x.RoomGroupId == model.RoomId)
            .Where(x => x.IsEnabled)
            .Where(x => x.AppDateId >= model.CheckInDate)
            .Where(x => x.AppDateId <= model.CheckOutDate)
            .Where(x => !x.IsNotSelled)
            .Where(
                x => x.RoomGroup!.FacilityRoomGroups!.Any(
                    facilityRoomGroup => facilityRoomGroup.FacilityId == model.FacilityId
                )
            )
            .Select(
                x => new RoomInventoryModel(
                    model.FacilityId,
                    x.RoomGroupId,
                    x.AppDateId,
                    x.SellNumber ?? 0,
                    x.RoomGroup!.ReservationPlanRoomGroupAppDates!
                        .Where(x => x.PlanId == model.PlanId)
                        .Where(t => t.BookingDateId == x.AppDateId)
                        .Where(t => t.Reservation!.UserCode != null)
                        .Count(
                            t => t.Reservation!.ReservationState == ReservationStatus.Confirmed
                                || t.Reservation!.ReservationState == ReservationStatus.Reserved
                                || t.Reservation!.ReservationState == ReservationStatus.Modified
                        ),
                    x.RoomGroup!.ReservationPlanRoomGroupAppDates!
                        .Where(x => x.PlanId == model.PlanId)
                        .Where(t => t.BookingDateId == x.AppDateId)
                        .Where(t => t.Reservation!.UserCode == null)
                        .Count(
                            t => t.Reservation!.ReservationState == ReservationStatus.Confirmed
                                || t.Reservation!.ReservationState == ReservationStatus.Reserved
                                || t.Reservation!.ReservationState == ReservationStatus.Modified
                        ),
                    x.RoomGroup!.ReservationPlanRoomGroupAppDates!
                        .Where(x => x.PlanId != model.PlanId)
                        .Where(t => t.BookingDateId == x.AppDateId)
                        .Where(t => t.Reservation!.UserCode == null)
                        .Count(
                            t => t.Reservation!.ReservationState == ReservationStatus.Confirmed
                                || t.Reservation!.ReservationState == ReservationStatus.Reserved
                                || t.Reservation!.ReservationState == ReservationStatus.Modified
                        )
                )
            )
            .AsSingleQuery();

        var roomInventory = await roomInventoryQueryable.ToListAsync(cancellationToken);

        List<RoomInventoryModel> availableRoomInventory = [];

        var dateIndex = 0;
        while (dateIndex < model.NumberOfNights)
        {
            var date = AppDate.GetId(
                model.CheckInDateTime.AddDays(dateIndex)
            );

            var roomInventoryForDate = roomInventory.Find(
                x => x.AppDateId == date
            );

            if (roomInventoryForDate is null)
            {
                availableRoomInventory.Add(
                    new RoomInventoryModel(
                        model.FacilityId,
                        model.RoomId,
                        date,
                        0,
                        0,
                        0,
                        0
                    )
                );
            }
            else
            {
                var availableQuantity = roomInventoryForDate.MaxQuantity - roomInventoryForDate.NumberOfReservedAnotherPlan;

                var maxQuantity = limit is not null && limit < roomInventoryForDate.MaxQuantity
                    ? Math.Min((int)limit, availableQuantity)
                    : availableQuantity;

                availableRoomInventory.Add(
                    new RoomInventoryModel(
                        model.FacilityId,
                        model.RoomId,
                        date,
                        maxQuantity,
                        roomInventoryForDate.NumberOfReservedForUser,
                        roomInventoryForDate.NumberOfReservedForGuest,
                        roomInventoryForDate.NumberOfReservedAnotherPlan
                    )
                );
            }

            dateIndex++;
        }

        return availableRoomInventory;
    }
}
