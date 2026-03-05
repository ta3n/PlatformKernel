using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.Site.WebAPI.Application.UserCases.Commands.Booking;

public class ChangePersonsBookingCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IBookingSearchService bookingSearchService,
    IBookingCheckAvailableService bookingCheckAvailableService,
    [FromKeyedServices("plan")] IBookingPriceService bookingPriceOfPlanService,
    [FromKeyedServices("room")] IBookingPriceService bookingPriceOfRoomService
) : ActionCommandHandlerBase<ChangePersonsBookingCommand, BookingPriceResponse>(unitOfWork, mapper)
{
    protected override async Task<BookingPriceResponse> HandleAsync(
        ChangePersonsBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        var facilityId = securityContextAccessor.GetFacilityIdSelected();
        var siteId = securityContextAccessor.GetSiteIdSelected();
        var planId = request.PlanId;
        var roomGroupId = request.RoomGroupId;
        var reservationDate = payload.CheckInDate;

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

        var useDailyPerson = planAvailableBooking.FacilityUseDailyPerson ?? false;
        var isHavePerson = payload.GuestsPerRoom.Exists(
            x => x.AppDateId
                == AppDate.GetId(
                    AppDate.GetDateTime(payload.CheckInDate).AddDays(payload.RestNumber)
                )
        );

        if (!useDailyPerson && isHavePerson)
        {
            throw new ReservationPriceException("Not change daily person");
        }

        var isInvalidChangePerson = await bookingCheckAvailableService.IsInvalidChangePersonsAsync(
            payload,
            planAvailableBooking,
            facilityId,
            roomGroupId,
            siteId,
            useDailyPerson,
            cancellationToken
        );

        if (isInvalidChangePerson)
        {
            throw new ReservationPriceException("GuestsPerRoom");
        }

        var result = planAvailableBooking.PlanType is PlanTypes.Combo
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

        return result!;
    }
}
