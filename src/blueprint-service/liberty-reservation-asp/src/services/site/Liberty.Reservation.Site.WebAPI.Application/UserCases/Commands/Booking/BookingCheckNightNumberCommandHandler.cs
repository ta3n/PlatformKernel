using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;

public class BookingCheckNightNumberCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IBookingSearchService bookingSearchService,
    IBookingCheckAvailableService bookingCheckAvailableService,
    [FromKeyedServices("plan")] IBookingPriceService bookingPriceOfPlanService,
    [FromKeyedServices("room")] IBookingPriceService bookingPriceOfRoomService
) : ActionCommandHandlerBase<BookingCheckNightNumberCommand, BookingPriceResponse>(unitOfWork, mapper)
{
    protected override async Task<BookingPriceResponse> HandleAsync(
        BookingCheckNightNumberCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;
        var siteId = securityContextAccessor.GetSiteIdSelected();
        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var planId = request.PlanId;
        var roomGroupId = request.RoomGroupId;
        var reservationDate = payload.CheckInDate;

        var isNightNumber = await bookingCheckAvailableService.IsNightNumberAsync(
            request.PlanId,
            request.RoomGroupId,
            siteId,
            payload.CheckInDate,
            payload.RestNumber,
            cancellationToken
        );

        if (!isNightNumber)
        {
            throw new BookingInvalidNightNumberException();
        }

        var planAvailableBooking = await bookingSearchService.GetBookingDataDetailByPlanAsync(
                new BookingPlanDetailRequest(
                    facilityId,
                    siteId,
                    planId,
                    roomGroupId,
                    reservationDate,
                    payload.RestNumber,
                    payload.Secret
                ),
                new BookingSearchPlanRequest
                {
                    CheckInDate = payload.CheckInDate,
                    CheckOutDate = payload.GetDateEndNight(),
                    RestNumber = payload.RestNumber,
                    RoomNumber = payload.RoomNumber,
                    Secret = payload.Secret,
                    GuestsPerRoom = payload.GuestsPerRoom.Select(
                        x => new PersonOfBookingSearchModel
                        {
                            AppDateId = x.AppDateId,
                            RestIndex = x.RestIndex,
                            RoomGroupIndex = x.RoomGroupIndex,
                            PersonAgeTypeId = x.PersonAgeTypeId,
                            Persons = x.Persons,
                            MalePersons = x.MalePersons,
                            FemalePersons = x.FemalePersons
                        }
                    ),
                    OptionItems = payload.OptionItems?.Select(
                        x => new OptionOfBookingSearchModel
                        {
                            AppDateId = x.AppDateId,
                            RoomGroupIndex = x.RoomGroupIndex,
                            OptionItemId = x.OptionItemId,
                            Number = x.Number
                        }
                    )
                },
                cancellationToken
            )
            ?? throw new PlanNotfoundException();

        var bookingPriceResponse = planAvailableBooking.PlanType is PlanTypes.Combo
            ? bookingPriceOfPlanService.GetBookingPrice(
                request.Payload,
                planAvailableBooking,
                roomGroupId
            )
            : bookingPriceOfRoomService.GetBookingPrice(
                request.Payload,
                planAvailableBooking,
                roomGroupId
            );

        return bookingPriceResponse!;
    }
}
