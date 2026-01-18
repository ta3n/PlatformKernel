using Liberty.Reservation.Application.Constants;
using Liberty.Reservation.Application.Contexts.DataContexts.Entities.Data;
using Liberty.Reservation.Application.Cqrs.BaseCommands;
using Liberty.Reservation.Application.Domains.Services.Interfaces;
using Liberty.Reservation.Application.Exceptions;
using Liberty.Reservation.Application.Models.Requests;
using Liberty.Reservation.Application.Models.Responses;
using Liberty.Reservation.User.Application.Auth;
using Liberty.UnitOfWork.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Liberty.Reservation.User.WebAPI.Application.UserCases.Commands.BookingReservation;

public class ChangePersonsBookingCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    ISecurityContextAccessor securityContextAccessor,
    IBookingDataAvailableService bookingDataAvailableService,
    IBookingCheckAvailableService bookingCheckAvailableService,
    IBookingSearchService bookingSearchService,
    [FromKeyedServices("plan")] IBookingPriceService bookingPriceOfPlanService,
    [FromKeyedServices("room")] IBookingPriceService bookingPriceOfRoomService
) : ActionCommandHandlerBase<ChangePersonsBookingCommand, BookingPriceResponse>(unitOfWork, mapper)
{
    protected override async Task<BookingPriceResponse> HandleAsync(
        ChangePersonsBookingCommand request,
        CancellationToken cancellationToken
    )
    {
        var userCode = securityContextAccessor.ApplicationUserKey;

        var reservation = await bookingDataAvailableService.GetReservationByUserCodeAsync(
            request.Id,
            userCode,
            cancellationToken
        );

        if (!reservation.CanModifyByUser(false))
        {
            throw new ReservationPersonAgeTypeException();
        }

        var payload = request.Payload;
        var useDailyPerson = reservation.Facility?.UseDailyPerson ?? false;
        var isHavePerson = payload.GuestsPerRoom.Exists(
            x => x.AppDateId
                == AppDate.GetId(
                    AppDate.GetDateTime(payload.CheckInDate).AddDays(payload.RestNumber)
                )
        );

        if (!useDailyPerson && isHavePerson)
        {
            throw new ReservationPersonAgeTypeException();
        }

        var isPlanTypeCombo = reservation.Plan!.PlanType == PlanTypes.Combo;
        request.Payload.ReservationId = request.Id;

        var planAvailableBooking = await bookingSearchService.GetBookingDataDetailByPlanAsync(
                new BookingPlanDetailRequest(
                    reservation.FacilityId,
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

        var isInvalidChangePerson = await bookingCheckAvailableService.IsInvalidChangePersonsAsync(
            request.Payload,
            planAvailableBooking,
            reservation.FacilityId,
            reservation.RoomGroupId,
            reservation.SiteId,
            useDailyPerson,
            cancellationToken
        );

        if (isInvalidChangePerson)
        {
            throw new ReservationPersonAgeTypeException();
        }

        try
        {
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
        catch (Exception ex) when (ex is ReservationPriceException)
        {
            throw new ReservationPersonAgeTypeException();
        }
    }
}
