using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingCheckNightNumberCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IBookingDataAvailableService bookingDataAvailableService,
    IBookingCheckAvailableService bookingCheckAvailableService,
    IBookingSearchService bookingSearchService,
    [FromKeyedServices("plan")] IBookingPriceService bookingPriceOfPlanService,
    [FromKeyedServices("room")] IBookingPriceService bookingPriceOfRoomService
) : ActionCommandHandlerBase<BookingCheckNightNumberCommand, BookingPriceResponse>(unitOfWork, mapper)
{
    protected override async Task<BookingPriceResponse> HandleAsync(
        BookingCheckNightNumberCommand request,
        CancellationToken cancellationToken
    )
    {
        var facilityId = securityContextAccessor.FacilityKey;
        var payload = request.Payload;
        var reservation = await bookingDataAvailableService.GetReservationByFacilityIdAsync(
            request.Id,
            facilityId,
            cancellationToken
        );

        var isNightNumber = await bookingCheckAvailableService.IsNightNumberAsync(
            reservation.Plan!.Id,
            reservation.RoomGroupId,
            reservation.SiteId,
            request.Payload.CheckInDate,
            request.Payload.RestNumber,
            cancellationToken
        );

        if (!isNightNumber)
        {
            throw new ReservationPriceException("Number of nights is not valid");
        }

        var isPlanTypeCombo = reservation.Plan!.PlanType == PlanTypes.Combo;
        request.Payload.ReservationId = request.Id;

        var planAvailableBooking = await bookingSearchService.GetBookingDataDetailByPlanAsync(
                new BookingPlanDetailRequest(
                    facilityId,
                    reservation.SiteId,
                    reservation.PlanId,
                    reservation.RoomGroupId,
                    reservation.CheckInDate,
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

        var bookingPriceResponse = isPlanTypeCombo
            ? bookingPriceOfPlanService.GetBookingPrice(
                request.Payload,
                planAvailableBooking,
                reservation.RoomGroupId
            )
            : bookingPriceOfRoomService.GetBookingPrice(
                request.Payload,
                planAvailableBooking,
                reservation.RoomGroupId
            );

        return bookingPriceResponse!;
    }
}
