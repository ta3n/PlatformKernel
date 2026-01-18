using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.Manager.Application.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Manager.WebAPI.Application.UserCases.Commands.BookingReservation;

public class BookingCheckRoomNumberCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IBookingDataAvailableService bookingDataAvailableService,
    IBookingCheckAvailableService bookingCheckAvailableService,
    IBookingSearchService bookingSearchService,
    [FromKeyedServices("plan")] IBookingPriceService bookingPriceOfPlanService,
    [FromKeyedServices("room")] IBookingPriceService bookingPriceOfRoomService
) : ActionCommandHandlerBase<BookingCheckRoomNumberCommand, BookingPriceResponse>(unitOfWork, mapper)
{
    protected override async Task<BookingPriceResponse> HandleAsync(
        BookingCheckRoomNumberCommand request,
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

        long reservationId = 0;
        if (request.Payload.RoomNumber > reservation.RoomNumber)
        {
            reservationId = request.Id;
        }

        var isRoomNumber = await bookingCheckAvailableService.IsRoomNumberAsync(
            reservation.Plan!.Id,
            reservation.RoomGroupId,
            request.Payload.CheckInDate,
            request.Payload.RoomNumber,
            request.Payload.RestNumber,
            reservationId,
            cancellationToken
        );

        var isRoomDecreased = reservation.RoomNumber >= request.Payload.RoomNumber;

        if (!isRoomNumber && !isRoomDecreased)
        {
            throw new ReservationRoomException();
        }

        var isPlanTypeCombo = reservation.Plan!.PlanType == PlanTypes.Combo;
        request.Payload.ReservationId = request.Id;

        try
        {
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
        catch (Exception ex) when (ex is PlanNotfoundException or ReservationPriceException)
        {
            throw new ReservationRoomException();
        }
    }
}
